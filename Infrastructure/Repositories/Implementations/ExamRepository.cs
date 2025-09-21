using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs.ExamDTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
        public async Task<int> AddExam(AddExamDTO dto)
        {
            Exam exam = new Exam
            {

                Name = dto.Name,
                Description = dto.Description,
                TotalQuestions = dto.TotalQuestions,
                Duration = dto.Duration,
                Tids = JsonConvert.SerializeObject(dto.Tids),
                DisplayedQuestions = dto.DisplayedQuestions,
                UserId = dto.userId,
                SubmittedForApproval = false

            };

            await _context.Exams.AddAsync(exam);
            return await _context.SaveChangesAsync();

        }
        public async Task<int> UpdateExam(int examId, UpdateExamDTO dto)
        {

            Exam ToBeUpdatedExam = await _context.Exams.FirstOrDefaultAsync(e => e.Eid == examId);

            if (ToBeUpdatedExam == null)
            {
                return 0;
            }

            if (dto.TotalQuestions != null && ToBeUpdatedExam.TotalQuestions != dto.TotalQuestions)
            {
                ToBeUpdatedExam.TotalQuestions = (int)dto.TotalQuestions;
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
                ToBeUpdatedExam.Tids = JsonConvert.SerializeObject(dto.Tids);
            }
            if (dto.DisplayedQuestions != null)
            {
                ToBeUpdatedExam.DisplayedQuestions = dto.DisplayedQuestions;
            }
            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteExam(int examId)
        {

            try
            {
                var exam = _context.Exams.FirstOrDefault(e => e.Eid == examId);
                if (exam != null && exam.ApprovalStatus == 1)
                {
                    exam.setApprovalStatus(0);
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
        public async Task<List<Exam>> GetExamsForExaminer(int userid)
        {
            var examdata = await _context.Exams.Where(e => e.UserId == userid).ToListAsync();
            return examdata;
        }
        public async Task<ExamWithQuestionsDTO> GetExamByIdForExaminer(int examId)
        {
            return await _context.Exams
                                .Where(e => e.Eid == examId)
                                .Select(e => new ExamWithQuestionsDTO
                                {
                                    Eid = e.Eid,
                                    ExamName = e.Name,
                                    TotalQuestions = e.TotalQuestions,
                                    ApprovalStatusOfExam = e.ApprovalStatus,
                                    Tids = e.Tids,
                                    Questions = e.Questions.Select(q => new QuestionDTO
                                    {
                                        Qid = q.Qid,
                                        Type = q.Type,
                                        Options = q.Options,
                                        Marks = q.Marks,
                                        QuestionText = q.Question1,
                                        CorrectOptions = q.CorrectOptions,
                                        ApprovalStatus = q.ApprovalStatus
                                    }).ToList()
                                })
                                .FirstOrDefaultAsync();


        }
        public async Task<List<Exam>> GetExamsAttemptedByUser(int UserId)
        {
            var user = await _context.Users
                   .Include(u => u.ExamUsers)
                   .ThenInclude(eu => eu.Results)
                   .FirstOrDefaultAsync(u => u.UserId == UserId);

            return (List<Exam>)user.ExamUsers;

        }
        public async Task<int> GetExamAttempts(int userId, int examId)
        {
            var result = await _context.Results.FirstOrDefaultAsync(r => r.UserId == userId && r.Eid == examId);
            if (result == null) return 0;
            return (int)result.Attempts;
        }

        //Student functions
        public async Task<List<GetExamDataDTO>> GetExams()
        {
            var examdata = await _context.Exams
                .Include(e => e.Results.Where(s => s.Eid == e.Eid))
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
            }).ToListAsync();
            if (examdata == null) return new List<GetExamDataDTO> { };
            else return examdata;
        }
        public async Task<StudentExamViewDTO> GetExams(int examId)
        {
            return (StudentExamViewDTO)await _context.Exams.Where(e => e.Eid == examId && e.ApprovalStatus == 1 && e.Questions != null)
                .Select(e => new StudentExamViewDTO
                {
                    Name = e.Name,
                    Description = e.Description,
                    DisplayedQuestions = e.DisplayedQuestions,
                    Duration = e.Duration,
                    Tids = e.Tids

                })
                .FirstOrDefaultAsync();
        }
        public async Task<StartExamResponseDTO> StartExam(int examId, int userId)
        {

            var hasUnprocessedAttempt = await _context.Responses
                   .AnyAsync(r => r.Eid == examId && r.UserId == userId && r.IsSubmittedFresh == true);

            if (hasUnprocessedAttempt)
            {
                Console.WriteLine("Processing the Previous Attempt.");
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC CreateExamResult @p0, @p1",
                    parameters: new object[] { examId, userId });
            }

            var allAttempts = await _context.Results.Where(r => r.UserId == userId && r.Eid == examId).MaxAsync(r => (int?)r.Attempts) ?? 0;
            if (allAttempts == 3)
            {
                return new StartExamResponseDTO();
            }

            var list = await _context.Exams.Include(e => e.Questions)
                .Where(e => e.Eid == examId && e.ApprovalStatus == 1)
                .Select(e => new StartExamResponseDTO
                {
                    EID = e.Eid,
                    TotalMarks = e.TotalMarks,
                    Duration = e.Duration,
                    Name = e.Name,
                    DisplayedQuestions = e.DisplayedQuestions,
                    Questions = e.Questions
                            .Where(q => q.ApprovalStatus == 1)
                            .Select(
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
                }).ToListAsync();
            var Data = list.FirstOrDefault();

            if (Data != null)
            {
                var random = new Random();
                var selectedQuestions = Data.Questions
                    .OrderBy(q => random.Next())
                    .Take((int)Data.DisplayedQuestions)
                    .ToList();

                var response = new StartExamResponseDTO
                {
                    EID = Data.EID,
                    TotalMarks = Data.TotalMarks,
                    Duration = Data.Duration,
                    Name = Data.Name,
                    DisplayedQuestions = Data.DisplayedQuestions,
                    Questions = selectedQuestions
                };

                return response;
            }


            return Data;
        }
        public async Task<int> SubmitExam(SubmittedExamDTO submittedData)
        {

            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Eid == submittedData.EID);
            if (exam == null)
            {
                return -1;
            }

            foreach (var responseDto in submittedData.Responses)
            {

                var existingResponse = await _context.Responses
                        .FirstOrDefaultAsync(r => r.UserId == submittedData.UserId && r.Eid == submittedData.EID && r.Qid == responseDto.Qid);

                if (existingResponse == null)
                {
                    var newResponse = new Response
                    {
                        Eid = submittedData.EID,
                        Qid = responseDto.Qid,
                        UserId = submittedData.UserId,
                        Resp = JsonConvert.SerializeObject(responseDto.Resp),
                        RespScore = null,
                        IsSubmittedFresh = true
                    };

                    await _context.Responses.AddAsync(newResponse);
                }
                else
                {
                    existingResponse.Resp = JsonConvert.SerializeObject(responseDto.Resp);
                    existingResponse.IsSubmittedFresh = true;
                    existingResponse.RespScore = null;

                }


            }

            int status = await _context.SaveChangesAsync();
            return status;

        }

        public async Task<int> SubmitExamForApproval(int examId)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Eid == examId);
            exam.SubmittedForApproval = true;
            return await _context.SaveChangesAsync();
        }

    }
}


