using Domain.Models;
using Infrastructure.DTOs.adminDTOs;
using Infrastructure.DTOs.ExamDTOs;
using Infrastructure.DTOs.QuestionFeedbackDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{

    public interface IAdminRepository
    {
        //Task<bool> RegisterAdminAsync(AdminCreateDto dto);

        Task<List<Exam>> ExamsToBeApprovedList(int userid);
        Task<int> ApproveExamAsync(ExamApprovalStatusDTO dto);
        //Task<bool> ReviewReportedQuestionAsync(int questionId);

        Task<List<QuestionReport>> GetAllReportedQuestionsAsync(int adminId);
        Task<Question?> GetReportedQuestionByIdAsync(int qid);
        Task<bool> UpdateReportedQuestionStatusAsync(QuestionReviewDTO dto);

        Task<string> BlockUserAsync(int userId);
        Task<IEnumerable<ExamFeedbackViewDTO>> GetExamFeedbacksAsync(int examId);

        Task<int> AddAdminRemarksAsync(int examId, string remarks);
        Task<List<ApproveTopicsDTO>> TopicsToBeApprovedAsync(int userId);

        Task<int> ApproveOrRejectTopic(int topicId, int userId, string Action);

        Task<Exam> GetExamWithQuestionsForAdminAsync(int userId);

    }

}

