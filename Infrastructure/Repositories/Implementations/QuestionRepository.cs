using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.DTOs.QuestionsDTO;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


            Question quest = new()
            {
                Type = question.type,
                Question1 = question.question,
                Marks = question.marks,
                Options = question.options,
                CorrectOptions = JsonConvert.SerializeObject(question.correctOptions),
                ApprovalStatus = question.ApprovalStatus
            };

            quest.Eid = eid;

            await _context.Questions.AddAsync(quest);
            return await _context.SaveChangesAsync();

        }

        public async Task<int> AddQuestionsToExam(List<AddQuestionDTO> questions, int eid)
        {
            List<Question> questionList = new();
            foreach (var question in questions)
            {
                Question quest = new()
                {
                    Eid = eid,
                    Type = question.type,
                    Question1 = question.question,
                    Marks = question.marks,
                    Options = question.options,
                    CorrectOptions = JsonConvert.SerializeObject(question.correctOptions),
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

        public async Task<int> UpdateQuestion(UpdateQuestionDTO question, int qid)
        {

            var updatedQuestion = new Question
            {
                Type = question.type,
                Question1 = question.question,
                Marks = question.marks,
                Options = question.options,
                CorrectOptions = JsonConvert.SerializeObject(question.correctOptions),
                ApprovalStatus = question.ApprovalStatus
            };

            var existingQuestion = await _context.Questions.FirstOrDefaultAsync(q => q.Qid == qid);
            if (existingQuestion == null)
                return 0;

            // Only update properties if they are not null
            if (updatedQuestion.Tid != null)
                existingQuestion.Tid = updatedQuestion.Tid;
            if (updatedQuestion.Eid != null)
                existingQuestion.Eid = updatedQuestion.Eid;
            if (updatedQuestion.Type != null)
                existingQuestion.Type = updatedQuestion.Type;
            if (updatedQuestion.Question1 != null)
                existingQuestion.Question1 = updatedQuestion.Question1;
            if (updatedQuestion.Marks != null)
                existingQuestion.Marks = updatedQuestion.Marks;
            if (updatedQuestion.Options != null)
                existingQuestion.Options = updatedQuestion.Options;
            if (updatedQuestion.CorrectOptions != null)
                existingQuestion.CorrectOptions = updatedQuestion.CorrectOptions;
            if (updatedQuestion.ApprovalStatus != null)
                existingQuestion.ApprovalStatus = updatedQuestion.ApprovalStatus;

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
