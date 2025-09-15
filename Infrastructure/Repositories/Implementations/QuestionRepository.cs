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
                return 0; // Or throw an exception if preferred

            // Update properties
            existingQuestion.Tid = updatedQuestion.Tid;
            existingQuestion.Eid = updatedQuestion.Eid;
            existingQuestion.Type = updatedQuestion.Type;
            existingQuestion.Question1 = updatedQuestion.Question1;
            existingQuestion.Marks = updatedQuestion.Marks;
            existingQuestion.Options = updatedQuestion.Options;
            existingQuestion.CorrectOptions = updatedQuestion.CorrectOptions;
            existingQuestion.ApprovalStatus = updatedQuestion.ApprovalStatus;

            return await _context.SaveChangesAsync();
        }
    }
}
