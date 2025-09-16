using Domain.Data;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;

        public QuestionRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<int> AddQuestion(Question question, int eid)
        {
            question.Eid = eid;

            await _context.Questions.AddAsync(question);
            return await _context.SaveChangesAsync();

        }

        public async Task<int> AddQuestionsToExam(List<Question> questions, int eid)
        {
            foreach (var question in questions)
            {
                question.Eid = eid;
            }
            await _context.Questions.AddRangeAsync(questions);
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

        public async Task<int> UpdateQuestion(Question updatedQuestion, int qid)
        {
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
    }
}
