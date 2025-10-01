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

        public async Task<ActionResult<StudentAnalyticsDTO>> GetStudentAnalytics(int userId)
        {

            var analyticsDto = new StudentAnalyticsDTO
            {
                UserId = userId,
                AverageScoreMultipleAttempts = await _context.Results
                    .Where(r => r.UserId == userId)
                    .GroupBy(r => r.Eid)
                    .Select(g => new AverageScoreMultipleAttempts
                    {
                        ExamId = g.Key,
                        ExamTitle = g.First().EidNavigation.Name,
                        AverageScore = (double)g.Average(r => r.Score),
                        TotalAttempts = g.Count()
                    }).ToListAsync(),
                TotalExamsTaken = await _context.Results
                    .Where(r => r.UserId == userId)
                    .Select(r => r.Eid)
                    .Distinct()
                    .CountAsync(),
                TotalQuestionsEncountered = await _context.Responses
                    .Where(rd => rd.UserId == userId)
                    .Select(rd => rd.Qid)
                    .Distinct()
                    .CountAsync(),
                ExamAttemptsRecords = new AttemptsRecords
                {
                    SingleAttempts = await _context.Results
                        .Where(r => r.UserId == userId)
                        .GroupBy(r => r.Eid)
                        .Where(g => g.Count() == 1)
                        .CountAsync(),
                    DoubleAttempts = await _context.Results
                        .Where(r => r.UserId == userId)
                        .GroupBy(r => r.Eid)
                        .Where(g => g.Count() == 2)
                        .CountAsync(),
                    TrippleAttempts = await _context.Results
                        .Where(r => r.UserId == userId)
                        .GroupBy(r => r.Eid)
                        .Where(g => g.Count() >= 3)
                        .CountAsync()
                },

                // Pseudocode:
                // 1. From Results, filter by userId.
                // 2. Join Results with Exams on Eid to get Tids (topic ids as string).
                // 3. Split Tids string into individual topic ids.
                // 4. Join topic ids with Topics table to get topic names.
                // 5. Group by topic id and topic name.
                // 6. For each topic, calculate average score from Results where Exam contains that topic.
                // 7. Return list of TopicWiseAverageScore.

                    OverallAverageScoreTopicWise = await (
                        from result in _context.Results
                        join exam in _context.Exams on result.Eid equals exam.Eid
                        where result.UserId == userId
                        from topicId in exam.Tids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        join topic in _context.Topics on int.Parse(topicId) equals topic.Tid
                        group new { result, topic } by new { topic.Tid, topic.Subject } into g
                        select new TopicWiseAverageScore
                        {
                            TopicId = g.Key.Tid,
                            Topic = g.Key.Subject,
                            AverageScore = (double)g.Average(x => x.result.Score ?? 0)
                        }
                    ).ToListAsync()
            };

            return analyticsDto;
        }

    }
}
