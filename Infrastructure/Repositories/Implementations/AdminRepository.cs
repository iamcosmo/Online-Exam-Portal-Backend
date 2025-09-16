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
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<bool> RegisterAdminAsync(AdminCreateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == dto.Email && u.Role == "Admin");
                
                // && !u.IsActive && !u.IsDeleted);

            if (user == null)
                return false;

            if (!SuperAdminRepository.ValidateToken(dto.Email, dto.Token))
                return false;

            user.FullName = dto.Name;
           user.Password = (dto.Password); // implement your hashing logic
            user.IsBlocked = true;

            SuperAdminRepository.InvalidateToken(dto.Email);
            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<bool> ApproveExamAsync(int examId,int status)
            {
            Console.WriteLine("status: "+status);
                var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Eid == examId);
                if (exam == null) return false;

            exam.setApprovalStatus();
            await _context.SaveChangesAsync();
            return true;

        }

        //public async Task<bool> ReviewReportedQuestionAsync(int questionId)
        //{
        //    var question = await _context.Questions.FindAsync(questionId);
        //    if (question == null) return false;

        //    question.ApprovalStatus = 1;
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        public async Task<bool> BlockUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsBlocked = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ExamFeedback>> GetExamFeedbacksAsync(int examId)
        {
            return await _context.ExamFeedbacks
                .Where(f => f.Eid == examId)
                .ToListAsync();
        }


        public async Task<List<QuestionReport>> GetAllReportedQuestionsAsync()
        {
            return await _context.QuestionReports
                .Include(r => r.QidNavigation)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<QuestionReport?> GetReportedQuestionByIdAsync(int qid)
        {
            return await _context.QuestionReports
                .Include(r => r.QidNavigation)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Qid == qid);
        }

        public async Task<bool> UpdateReportedQuestionStatusAsync(int qid, int status)
        {
            var question = await _context.Questions.FindAsync(qid);
            if (question == null) return false;

            question.ApprovalStatus = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }

}

