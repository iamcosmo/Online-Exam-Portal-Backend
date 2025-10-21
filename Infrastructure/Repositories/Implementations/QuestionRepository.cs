using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs.QuestionsDTO;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Infrastructure.Repositories.Implementations
{
    public class QuestionRepository : IQuestionRepository
    {

        private readonly AppDbContext _context;

        public QuestionRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<int> AddQuestion(AddQuestionDTO question, int eid)
        {

            bool topicExistsAndApproved = await _context.Topics.AnyAsync(t => t.Tid == question.Tid && t.ApprovalStatus == 1);
            if (!topicExistsAndApproved)
                return 0;

            int? MarksPerQuestion = await _context.Exams
                .Where(e => e.Eid == eid)
                .Select(e => e.MarksPerQuestion)
                .FirstOrDefaultAsync();

            Question quest = new()
            {
                Type = question.type,
                Question1 = question.question,
                Marks = MarksPerQuestion,
                Options = question.options,
                Tid = question.Tid,
                CorrectOptions = JsonConvert.SerializeObject(question.correctOptions),
                ApprovalStatus = question.ApprovalStatus
            };

            quest.Eid = eid;

            await _context.Questions.AddAsync(quest);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> AddBatchQuestionsToExam(AddQuestionsByBatchDTO questions, int eid)
        {
            int? MarksPerQuestion = await _context.Exams
                .Where(e => e.Eid == eid)
                .Select(e => e.MarksPerQuestion)
                .FirstOrDefaultAsync();

            List<Question> questionList = new();
            foreach (var question in questions.Questions)
            {
                Question quest = new()
                {
                    Eid = eid,
                    Tid = questions.Tid,
                    Type = question.Type,
                    Question1 = question.Question,
                    Marks = MarksPerQuestion,
                    Options = question.Options,
                    CorrectOptions = JsonConvert.SerializeObject(question.CorrectOptions),
                    ApprovalStatus = question.ApprovalStatus
                };
                questionList.Add(quest);
            }

            await _context.Questions.AddRangeAsync(questionList);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Question>> GetQuestionsByExamId(int examId)
        {
            List<Question> questions = await _context.Questions.Where(q => q.Eid == examId).ToListAsync();
            return questions;
        }

        public Question GetQuestionById(int questionId)
        {
            return _context.Questions.FirstOrDefault(q => q.Qid == questionId);

        }

        public async Task<(List<ListQuestionsDTO> Questions, int TotalCount)> GetQuestionsByExaminerID(int examinerId, int page,int pageSize)
        {
            // 1. Define the base query (without pagination)
            var baseQuery = _context.Questions
                .Include(q => q.EidNavigation)
                .Where(q => q.EidNavigation != null && q.EidNavigation.UserId == examinerId);

            // 2. Get the total count of all matching questions
            var totalCount = await baseQuery.CountAsync();

            // 3. Apply Skip and Take for pagination
            var questions = await baseQuery
                .Skip((page - 1) * pageSize) // Skip items from previous pages
                .Take(pageSize)              // Take only the current page's items
                .Select(q => new ListQuestionsDTO
                {
                    QuestionName = q.Question1,
                    QuestionId = q.Qid,
                    QuestionType = q.Type
                })
                .ToListAsync();

            return (questions, totalCount);
        }

        public async Task<int> UpdateQuestion(UpdateQuestionDTO question, int qid)
        {

            var updatedQuestion = new Question
            {
                Type = question.type,
                Question1 = question.question,
                Options = question.options,
                CorrectOptions = JsonConvert.SerializeObject(question.correctOptions),
                ApprovalStatus = question.ApprovalStatus
            };

            var existingQuestion = await _context.Questions.FirstOrDefaultAsync(q => q.Qid == qid);
            if (existingQuestion == null)
                return 0;

            // Only update properties if they are not null

            existingQuestion.Type = question.type;
            existingQuestion.Question1 = question.question;
            existingQuestion.Options = question.options;
            existingQuestion.CorrectOptions = JsonConvert.SerializeObject(question.correctOptions);
            existingQuestion.ApprovalStatus = question.ApprovalStatus;
            //if (updatedQuestion.Tid != null)
            //    existingQuestion.Tid = updatedQuestion.Tid;
            //if (updatedQuestion.Eid != null)
            //    existingQuestion.Eid = updatedQuestion.Eid;
            //if (updatedQuestion.Type != null)
            //    existingQuestion.Type = updatedQuestion.Type;
            //if (updatedQuestion.Question1 != null)
            //    existingQuestion.Question1 = updatedQuestion.Question1;
            //if (updatedQuestion.Marks != null)
            //    existingQuestion.Marks = updatedQuestion.Marks;
            //if (updatedQuestion.Options != null)
            //    existingQuestion.Options = updatedQuestion.Options;
            //if (updatedQuestion.CorrectOptions != null)
            //    existingQuestion.CorrectOptions = updatedQuestion.CorrectOptions;
            //if (updatedQuestion.ApprovalStatus != null)
            //    existingQuestion.ApprovalStatus = updatedQuestion.ApprovalStatus;

            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteQuestion(int qid)
        {
            // Find the question
            var question = await _context.Questions.FirstOrDefaultAsync(q => q.Qid == qid);
            if (question == null)
                return 0;

            var exam = new Exam();
            // Check and update Exam if approvalStatus is 1
            if (question.Eid.HasValue)
            {
                exam = await _context.Exams.FirstOrDefaultAsync(e => e.Eid == question.Eid.Value);
                if (exam != null && exam.ApprovalStatus == 1)
                {
                    exam.ApprovalStatus = 0;
                    if (question.Marks.HasValue && exam.TotalMarks.HasValue)
                        exam.TotalMarks -= question.Marks.Value;
                }


            }

            // Delete related QuestionReports
            var questionReports = await _context.QuestionReports.Where(qr => qr.Qid == qid).ToListAsync();
            if (questionReports.Any())
                _context.QuestionReports.RemoveRange(questionReports);

            // Delete related Responses
            var responses = await _context.Responses.Where(r => r.Qid == qid).ToListAsync();
            if (responses.Any())
                _context.Responses.RemoveRange(responses);

            // Delete the Question itself
            _context.Questions.Remove(question);

            return await _context.SaveChangesAsync();
        }
    }
}
