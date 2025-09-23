using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs.adminDTOs;
using Infrastructure.DTOs.ExamDTOs;
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

        public async Task<List<Exam>> ExamsToBeApprovedList()
        {
            List<Exam> ExamList = new List<Exam> { };
            ExamList = await _context.Exams.Include(q => q.Questions).Where(e => e.SubmittedForApproval == true).ToListAsync();
            return ExamList;

        }
        public async Task<int> ApproveExamAsync(ExamApprovalStatusDTO dto)
        {

            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Eid == dto.eid);
            if (exam == null) return 0;

            if (dto.action == "approve") { exam.setApprovalStatus(1); }
            else if (dto.action == "reject") { exam.setApprovalStatus(0); }
            //exam.ApprovedByUserId = dto.userId;
            exam.SubmittedForApproval = false;
            return await _context.SaveChangesAsync();

        }
        public async Task<int> BlockUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return 0;

            if (user.Role == "Admin") { return -1; }

            user.IsBlocked = true;
            await _context.SaveChangesAsync();
            return 1;
        }

        public async Task<IEnumerable<ExamFeedbackViewDTO>> GetExamFeedbacksAsync(int examId)
        {
            return await _context.ExamFeedbacks
                .Where(f => f.Eid == examId)
                .Select(ef => new ExamFeedbackViewDTO { Eid = ef.Eid, Feedback = ef.Feedback, StudentId = ef.UserId })
                .ToListAsync();
        }


        public List<QuestionReport> GetAllReportedQuestionsAsync()
        {
            return _context.QuestionReports.ToList();
        }

        public QuestionReport? GetReportedQuestionByIdAsync(int qid)
        {
            return _context.QuestionReports.FirstOrDefault(r => r.Qid == qid);
        }

        public async Task<bool> UpdateReportedQuestionStatusAsync(int qid, int status)
        {
            var question = await _context.Questions.FindAsync(qid);
            if (question == null) return false;

            question.ApprovalStatus = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> AddAdminRemarks(int examId, string remarks)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Eid == examId);
            exam.AdminRemarks = remarks;
            exam.SubmittedForApproval = false;
            return await _context.SaveChangesAsync();

        }

        public async Task<List<ApproveTopicsDTO>> TopicsToBeApprovedAsync(int userId)
        {
            return await _context.Topics.Where(t => t.SubmittedForApproval == true && t.ApprovedByUserId == userId)
                .Select(t => new ApproveTopicsDTO { Id = t.Tid, TopicName = t.Subject }).ToListAsync();
        }

        public async Task<int> ApproveOrRejectTopic(int topicId, int userId)
        {
            Topic? topic = await _context.Topics.FirstOrDefaultAsync(t => t.Tid == topicId);

            if (topic != null)
            {
                topic.SetApprovalStatus(1);
                topic.SubmittedForApproval = false;
                await _context.SaveChangesAsync();
            }
            return topic != null ? 1 : 0;
        }
    }

}

