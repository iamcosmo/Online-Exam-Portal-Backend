using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.DTOs.QuestionFeedbackDTO;
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
        public string AddQuestionFeedbackDTO(QuestionReport qFeedback)
        {
            bool exists = _context.QuestionReports
                .Any(qr => qr.Qid == qFeedback.Qid && qr.UserId == qFeedback.UserId);

            if (exists)          
                return "You have alreayd Reported this!";            

            _context.QuestionReports.Add(qFeedback);
            return _context.SaveChanges()>0?"Reported Successfully!!":"There was an error Reporting this Question!";
        }
        public async Task<List<GetQuestionFeedback>> GetFeedbackByQuestionId(int qid)
        {
            List<QuestionReport> questionReports = await _context.QuestionReports
                    .Where(QuestionReport => QuestionReport.Qid == qid)
                    .ToListAsync();

            List<GetQuestionFeedback> feedbacks = questionReports.Select(r => new GetQuestionFeedback
            {
                qId = r.Qid,
                feedback = r.Feedback ?? string.Empty,
                userId = r.UserId
            }).ToList();

            // Fix: Return an empty list if no feedbacks found, not a string
            return feedbacks;
        }

        public async Task<List<GetQuestionFeedback>> GetAllFeedbacks()
        {
            List<QuestionReport> reports = await _context.QuestionReports.ToListAsync();

            List<GetQuestionFeedback> feedbacks = reports.Select(r => new GetQuestionFeedback
            {
                qId = r.Qid,
                feedback = r.Feedback ?? string.Empty,
                userId = r.UserId
            }).ToList();

            return feedbacks;
        }

        public async Task<List<GetQuestionFeedback>> GetAllFeedbacks(int userId)
        {
            List<QuestionReport> reports = await _context.QuestionReports
                .Where(r => r.UserId == userId)
                .ToListAsync();

            List<GetQuestionFeedback> feedbacks = reports.Select(r => new GetQuestionFeedback
            {
                qId = r.Qid,
                feedback = r.Feedback ?? string.Empty,
                userId = r.UserId
            }).ToList();

            return feedbacks;
        }
        public async Task<int> UpdateQuestionFeedback(string updatedFeedback, int qid, int uId)
        {
            
            var existingReport = await _context.QuestionReports
                .FirstOrDefaultAsync(qr => qr.Qid == qid && qr.UserId == uId);

            
            if (existingReport != null)
            {
                existingReport.Feedback = updatedFeedback;
               
                return await _context.SaveChangesAsync();
            }

            return 0;
        }
    }
}
