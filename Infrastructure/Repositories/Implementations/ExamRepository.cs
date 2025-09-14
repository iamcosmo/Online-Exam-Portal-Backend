using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Repositories.Implementations
{
    public class ExamRepository : IExamRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExamRepository> _log;
        private readonly IConfiguration _config;

        public ExamRepository(AppDbContext dbContext, ILogger<ExamRepository> logger, IConfiguration configuration)
        {
            _context = dbContext;
            _log = logger;
            _config = configuration;
        }

        //CRUD
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
        public int DeleteExam(int examId)
        {

            var responses = _context.Responses.Where(r => r.Eid == examId);
            var results = _context.Results.Where(r => r.Eid == examId);
            var questions = _context.Questions.Where(q => q.Eid == examId);
            var exam = _context.Exams.FirstOrDefault(e => e.Eid == examId);

            if (exam == null) return 0;

            _context.Responses.RemoveRange(responses);
            _context.Results.RemoveRange(results);
            _context.Questions.RemoveRange(questions);
            _context.Exams.Remove(exam);

            return _context.SaveChanges();

            /*
                ALTER TABLE Question
                ADD CONSTRAINT FK_Question_Exam
                FOREIGN KEY (Eid) REFERENCES Exams(EID)
                ON DELETE CASCADE;
            */

        }
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
                AttemptNo = e.Results
                                .Where(r => r.Eid == e.Eid && r.UserId == e.UserId)
                                .Max(r => (int?)r.Attempts) ?? 0
            }).ToList();
            if (examdata == null) return new List<GetExamDataDTO> { };
            else return examdata;
        }
        public List<Exam> GetExamsForExaminer(int userid)
        {
            var examdata = _context.Exams.Where(e => e.UserId == userid).ToList();
            return examdata;
        }
        public StudentExamViewDTO GetExamById(int examId)
        {
            return (StudentExamViewDTO)_context.Exams.Where(e => e.Eid == examId && e.ApprovalStatus == 1)
                .Select(e => new StudentExamViewDTO
                {
                    Name = e.Name,
                    Description = e.Description,
                    DisplayedQuestions = e.DisplayedQuestions,
                    Duration = e.Duration,
                    Tids = e.Tids

                });
        }
        public Exam GetExamByIdForExaminer(int examId)
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

        //Student functions
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
                    RespScore = null,
                    IsSubmittedFresh = true
                };

                _context.Responses.Add(response);
            }

            int status = _context.SaveChanges();
            _log.LogInformation("status=" + status);
            return status;

        }
        public List<ExamResultsDTO> ViewExamResults(int examid, int userid)
        {
            List<ExamResultsDTO> results = _context.Results.Where(r => r.Eid == examid && r.UserId == userid).Select(r => new ExamResultsDTO
            {
                UserId = r.UserId,
                Eid = r.Eid,
                Attempts = r.Attempts,
                Score = r.Score
            }).ToList();

            return results;
        }
        //Using ADO.NET
        public int CreateExamResults(int examid, int userid)
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("CreateExamResult", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExamId", examid);
                    command.Parameters.AddWithValue("@UserId", userid);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0 ? 1 : 0;
                }
            }
        }
    }
}


