using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
namespace Infrastructure.Repositories.Implementations
{
    public class ExamRepository : IExamRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public ExamRepository(AppDbContext dbContext, IConfiguration configuration)
        {
            _context = dbContext;
            _config = configuration;
        }

        //CRUD
        public async Task<int> AddExam(Exam exam)
        {
            await _context.Exams.AddAsync(exam);
            return await _context.SaveChangesAsync();

        }
        public int UpdateExam(int examId, UpdateExamDTO dto)
        {

            Exam ToBeUpdatedExam = _context.Exams.FirstOrDefault(e => e.Eid == examId);
            if (dto.TotalMarks != 0 && ToBeUpdatedExam.TotalMarks != dto.TotalMarks)
            {
                ToBeUpdatedExam.TotalMarks = dto.TotalMarks;
            }

            if (dto.Description != null)
            {
                ToBeUpdatedExam.Description = dto.Description;
            }
            if (dto.Duration != null)
            {
                ToBeUpdatedExam.Duration = dto.Duration;
            }
            if (dto.Tids != null)
            {
                ToBeUpdatedExam.Tids = dto.Tids;
            }
            if (dto.DisplayedQuestions != null)
            {
                ToBeUpdatedExam.DisplayedQuestions = dto.DisplayedQuestions;
            }
            return _context.SaveChanges();
        }
        public int DeleteExam(int examId)
        {

            try
            {
                var exam = _context.Exams.FirstOrDefault(e => e.Eid == examId);
                if (exam != null && exam.ApprovalStatus == 1)
                {
                    exam.setApprovalStatus();
                    return _context.SaveChanges();
                }
                else if (exam == null)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -99;
            }

        }
        public List<GetExamDataDTO> GetExams()
        {
            var examdata = _context.Exams.Include(e => e.Results.Where(s => s.Eid == e.Eid))
                .Where(e => e.Questions != null && e.ApprovalStatus == 1)
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
        public StudentExamViewDTO GetExams(int examId)
        {
            return (StudentExamViewDTO)_context.Exams.Where(e => e.Eid == examId && e.ApprovalStatus == 1 && e.Questions != null)
                .Select(e => new StudentExamViewDTO
                {
                    Name = e.Name,
                    Description = e.Description,
                    DisplayedQuestions = e.DisplayedQuestions,
                    Duration = e.Duration,
                    Tids = e.Tids

                })
                .FirstOrDefault();
        }
        public List<Exam> GetExamsForExaminer(int userid)
        {
            var examdata = _context.Exams.Where(e => e.UserId == userid).ToList();
            return examdata;
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
            if (result == null) return 0;
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
            return status;

        }

    }
}


