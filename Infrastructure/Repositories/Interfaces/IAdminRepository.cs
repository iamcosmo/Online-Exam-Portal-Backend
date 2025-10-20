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

        Task<int> BlockUserAsync(int userId);
        Task<IEnumerable<ExamFeedbackViewDTO>> GetExamFeedbacksAsync(int examId);

        //Task<List<Question>> GetAllReportedQuestionsAsync();

        Task<int> AddAdminRemarks(int examId, string remarks);
        Task<List<ApproveTopicsDTO>> TopicsToBeApprovedAsync(int userId);

        Task<int> ApproveOrRejectTopic(int topicId, int userId, string Action);

    }

}

