using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories.Implementations
{
    public class ExamRepository : IExamRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger _log;

        public ExamRepository(AppDbContext dbContext, ILogger logger)
        {
            _context = dbContext;
            _log = logger;
        }

        public async Task<int> AddExam(Exam exam)
        {
            await _context.Exams.AddAsync(exam);
            return await _context.SaveChangesAsync();

        }
        public int UpdateExam(Exam exam)
        {
            Exam ToBeUpdatedExam = _context.Exams.FirstOrDefault(e => e.Eid == exam.Eid);
            if (ToBeUpdatedExam.TotalMarks != exam.TotalMarks)
            {
                ToBeUpdatedExam.TotalMarks = exam.TotalMarks;
            }
            else if (ToBeUpdatedExam.TotalQuestions != exam.TotalQuestions)
            {
                ToBeUpdatedExam.TotalQuestions = exam.TotalQuestions;
            }
            return _context.SaveChanges();
        }
        public int DeleteExam(int examId) { return 1; }
        public List<GetExamDataDTO> GetExams()
        {
            var examdata = _context.Exams.Include(e => e.Results.Where(s => s.Eid == e.Eid))
            .Select(e => new GetExamDataDTO
            {
                eid = e.Eid,
                name = e.Name,
                description = e.Description,
                totalMarks = e.TotalMarks ?? 0m,
                duration = e.Duration ?? 0m,
                Tids = e.Tids,
                displayedQuestions = e.DisplayedQuestions ?? 0,
                AttemptNo = (int)e.Results.Where(r => r.Eid == e.Eid).Select(r => r.Attempts).First()

            }).ToList();
            if (examdata == null) return new List<GetExamDataDTO> { };
            else return examdata;
        }

        public Exam GetExamById(int examId)
        {
            return _context.Exams.FirstOrDefault(e => e.Eid == examId);
        }

        public List<Exam> GetExamsAttemptedByUser(int UserId)
        {
            var user = _context.Users
                   .Include(u => u.ExamUsers)
                   .ThenInclude(eu => eu.Results)
                   .FirstOrDefault(u => u.UserId == UserId);

            return (List<Exam>)user.ExamUsers;

        }
        public int GetExamAttempts(int userId, int examId)
        {
            var result = _context.Results.FirstOrDefault(r => r.UserId == userId && r.Eid == examId);
            return (int)result.Attempts;
        }

        public StartExamResponseDTO StartExam(int examId)
        {
            var Data = _context.Exams.Include(e => e.Questions)
                .Where(e => e.Eid == examId)
                .Select(e => new StartExamResponseDTO
                {
                    EID = e.Eid,
                    TotalMarks = e.TotalMarks,
                    Duration = e.Duration,
                    Name = e.Name,
                    DisplayedQuestions = e.DisplayedQuestions,
                    Questions = e.Questions.Select(
                            q => new StartExamQuestionsDTO
                            {
                                Qid = q.Qid,
                                Type = q.Type,
                                QuestionName = q.Question1,
                                Marks = q.Marks,
                                Options = q.Options,
                                ApprovalStatus = q.ApprovalStatus
                            }
                    ).ToList(),
                }).ToList()
                .FirstOrDefault();

            return Data;
        }

        public int SubmitExam(SubmittedExamDTO submittedData)
        {

            var exam = _context.Exams.FirstOrDefault(e => e.Eid == submittedData.EID);
            if (exam == null)
            {
                return -1;
            }

            exam.TotalMarks = submittedData.TotalMarks;
            exam.Duration = submittedData.Duration;
            exam.DisplayedQuestions = submittedData.DisplayedQuestions;
            exam.Name = submittedData.Name;

            foreach (var responseDto in submittedData.Responses)
            {
                var response = new Response
                {
                    Eid = submittedData.EID,
                    Qid = responseDto.Qid,
                    UserId = submittedData.UserId,
                    Resp = responseDto.Resp,
                    RespScore = null
                };

                _context.Responses.Add(response);
            }

            int status = _context.SaveChanges();
            _log.LogInformation("status=" + status);
            return status;

        }

    }
}


