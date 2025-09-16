using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class QuestionFeedbackRepository:IQuestionFeedbackRepository
    {

        private readonly AppDbContext _context;

        public QuestionFeedbackRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }
        public int AddQuestionFeedbackDTO(QuestionReport qFeedback)
        {
            _context.QuestionReports.Add(qFeedback);
            return _context.SaveChanges();
            
        }
        public async Task<string> GetFeedbackByQuestionId(int qid)
        {
            var questionReport = await _context.QuestionReports
            .FirstOrDefaultAsync(QuestionReport => QuestionReport.Qid == qid);

            if (questionReport != null)
            {
                return questionReport.Feedback; 
            }

            return "No feedback found for this question.";
        }
        public async Task<int> UpdateQuestionFeedback(QuestionReport qFeedback, int qid)
        {
            
            var existingReport = await _context.QuestionReports.FirstOrDefaultAsync(qr => qr.Qid == qid);

            
            if (existingReport != null)
            {
                existingReport.Feedback = qFeedback.Feedback;
               
                return await _context.SaveChangesAsync();
            }

            return 0;
        }
    }
}
