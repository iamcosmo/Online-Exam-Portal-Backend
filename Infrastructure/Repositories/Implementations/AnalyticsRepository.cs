using Domain.Data;
using Infrastructure.DTOs.Analytics;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly AppDbContext _context;

        public AnalyticsRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult<AdminAnalyticsDto>> GetAdminAnalytics()
        {
            var dto = new AdminAnalyticsDto
            {
                TotalExams = await _context.Exams.CountAsync(),
                TotalQuestions = await _context.Questions.CountAsync(),
                TotalStudents = await _context.Users.Where(s => s.Role == "Student").CountAsync(),
                TotalExaminers = await _context.Users.Where(e => e.Role == "Examiner").CountAsync(),
                BlockedExaminers = await _context.Users.Where(e => e.IsBlocked == true && e.Role == "Examiner").CountAsync(),
                TopExams = await _context.Results
                   .GroupBy(r => r.Eid)
                   .Select(g => new TopExamDto
                   {
                       ExamId = g.Key,
                       ExamTitle = g.First().EidNavigation.Name,
                       AverageScore = (double)g.Average(r => r.Score)
                   })
                   .OrderByDescending(x => x.AverageScore)
                   .Take(5)
                   .ToListAsync()
            };

            return dto != null ? dto : new AdminAnalyticsDto();
        }

        public async Task<ActionResult<ExaminerAnalyticsDto>> GetExaminerAnalytics(int examinerId)
        {
            var dto = new ExaminerAnalyticsDto
            {
                TotalExamsCreated = await _context.Exams.Where(e => e.Eid == examinerId).CountAsync(),

                AverageScoresPerExam = await _context.Results
                    .Where(r => r.EidNavigation.UserId == examinerId)
                    .GroupBy(r => r.Eid)
                    .Select(g => new ExamScoreDto
                    {
                        ExamId = g.Key,
                        ExamTitle = g.First().EidNavigation.Name,
                        AverageScore = (double)g.Average(r => r.Score)
                    }).ToListAsync(),

                QuestionApprovalStats = await _context.Questions
                    .Where(q => q.EidNavigation.UserId == examinerId)
                    .GroupBy(q => q.ApprovalStatus == 1)
                    .Select(g => new QuestionApprovalDto
                    {
                        IsApproved = g.Key,
                        Count = g.Count()
                    }).ToListAsync(),

                StudentParticipation = await _context.Results
                    .Where(r => r.EidNavigation.UserId == examinerId)
                    .GroupBy(r => r.Eid)
                    .Select(g => new ExamParticipationDto
                    {
                        ExamId = g.Key,
                        ExamTitle = g.First().EidNavigation.Name,
                        StudentCount = g.Select(r => r.UserId).Distinct().Count()
                    }).ToListAsync()
            };

            return dto != null ? dto : new ExaminerAnalyticsDto();
        }

    }
}
