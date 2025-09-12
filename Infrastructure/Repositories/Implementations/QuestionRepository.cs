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

        public async Task<int> UpdateQuestion(Question qid)
        {
            Question ToBeUpdatedQuestion = _context.Questions.FirstOrDefault(q => q.Qid == qid.Qid);
            return await _context.SaveChangesAsync();
        }
    }
}
