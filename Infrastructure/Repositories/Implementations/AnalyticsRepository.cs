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

                //OverallAverageScoreTopicWise = await (
                //    from result in _context.Results
                //    join exam in _context.Exams on result.Eid equals exam.Eid
                //    where result.UserId == userId
                //    from topicId in exam.Tids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                //    join topic in _context.Topics on int.Parse(topicId) equals topic.Tid
                //    group new { result, topic } by new { topic.Tid, topic.Subject } into g
                //    select new TopicWiseAverageScore
                //    {
                //        TopicId = g.Key.Tid,
                //        Topic = g.Key.Subject,
                //        AverageScore = (double)g.Average(x => x.result.Score ?? 0)
                //    }
                //).ToListAsync()

                OverallAverageScoreTopicWise = await Task.Run(() => // Wrap in Task.Run for async compatibility if needed
                {
                    // 1. Database Query: Pull Results and Exams for the User
                    var rawAnalyticsData = (
                        from result in _context.Results
                        join exam in _context.Exams on result.Eid equals exam.Eid
                        where result.UserId == userId
                        select new
                        {
                            result.Score,
                            exam.Tids // Get the JSON string
                        }
                    ).AsEnumerable(); // Force execution of DB query here. The rest runs in memory.

                    // 2. Client-Side Processing: Deserialize JSON and Flatten Data
                    var topicScores = rawAnalyticsData
                        .SelectMany(x =>
                        {
                            // Safely deserialize the JSON string into an array of integers (Topic IDs)
                            var topicIds = JsonConvert.DeserializeObject<List<int>>(x.Tids ?? "[]");

                            // Create a flat list of {Score, TopicId} pairs
                            return topicIds.Select(tid => new { Score = x.Score, TopicId = tid });
                        })
                        .ToList(); // Convert to List to use the data efficiently

                    // 3. Final Grouping and Averaging (In-Memory)
                    var allTopics = _context.Topics.ToList(); // Load ALL topics into memory for joining

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
            return await _context.Questions.CountAsync(q => q.ApprovalStatus == 1);
        }
    }
}
