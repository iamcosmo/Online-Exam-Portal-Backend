using Domain.Data;
using Infrastructure.DTOs.Analytics;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                TotalExamsCreated = await _context.Exams.Where(e => e.UserId == examinerId).CountAsync(),

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
                    ,
                TopicApprovalStats = await _context.Topics
                    .Where(t => t.ExaminerId == examinerId)
                    .GroupBy(t => new { t.Tid, t.Subject, IsApproved = t.ApprovalStatus == 1 })
                    .Select(g => new TopicApprovalDto
                    {
                        TopicId = g.Key.Tid,
                        Subject = g.Key.Subject,
                        IsApproved = g.Key.IsApproved,
                        Count = g.Count()
                    }).ToListAsync(),

                AvgTopicScores = await _context.Responses
                    .Include(r => r.QidNavigation)
                    .ThenInclude(q => q.TidNavigation)
                    .Where(r => r.QidNavigation.TidNavigation.ExaminerId == examinerId)
                    .GroupBy(r => new { r.QidNavigation.Tid, r.QidNavigation.TidNavigation.Subject })
                    .Select(g => new TopicScoreDto
                    {
                        TopicId = g.Key.Tid,
                        Subject = g.Key.Subject,
                        AverageScore = g.Average(r => r.RespScore ?? 0)

                    }).ToListAsync()
                    ,


                TopicQuestionCounts = await _context.Questions
                    .Include(q => q.TidNavigation)
                    .Where(q => q.TidNavigation.ExaminerId == examinerId)
                    .GroupBy(q => new { q.Tid, q.TidNavigation.Subject })
                    .Select(g => new TopicQuestionCountDto
                    {
                        TopicId = g.Key.Tid,
                        Subject = g.Key.Subject,
                        QuestionCount = g.Count()
                    }).ToListAsync()

            };

            return dto != null ? dto : new ExaminerAnalyticsDto();
        }

        public async Task<ActionResult<StudentAnalyticsDTO>> GetStudentAnalytics(int userId)
        {

            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);

            if (!userExists)
                return new NotFoundResult();

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
                TotalExamsTaken = await _context.Responses
                    .Where(rd => rd.UserId == userId)
                    .Select(rd => rd.Eid)
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

               

                OverallAverageScoreTopicWise = await Task.Run(() =>
                {
                   
                    var rawAnalyticsData = (
                        from result in _context.Results
                        join exam in _context.Exams on result.Eid equals exam.Eid
                        where result.UserId == userId
                        select new
                        {
                            result.Score,
                            exam.Tids 
                        }
                    ).AsEnumerable(); 
                    var topicScores = rawAnalyticsData
                        .SelectMany(x =>
                        {                            
                            var topicIds = JsonConvert.DeserializeObject<List<int>>(x.Tids ?? "[]");                             
                            return topicIds.Select(tid => new { Score = x.Score, TopicId = tid });
                        })
                        .ToList(); 

                   
                    var allTopics = _context.Topics.ToList();

                    return topicScores
                        .Join(
                            allTopics,
                            ts => ts.TopicId,
                            topic => topic.Tid,
                            (ts, topic) => new { ts.Score, topic.Tid, topic.Subject }
                        )
                        .GroupBy(x => new { x.Tid, x.Subject })
                        .Select(g => new TopicWiseAverageScore
                        {
                            TopicId = g.Key.Tid,
                            Topic = g.Key.Subject,
                            AverageScore = (double)g.Average(x => x.Score ?? 0)
                        })
                        .ToList();
                }),
            };

            return analyticsDto;
        }

        public async Task<int> GetTotalActiveExams()
        {
            return await _context.Exams.CountAsync(e => e.ApprovalStatus == 1);
        }

        public async Task<int> GetTotalActiveQuestions()
        {
            return await _context.Questions.CountAsync(q => q.ApprovalStatus == 1 && q.EidNavigation.ApprovalStatus == 1);
        }
    }
}
