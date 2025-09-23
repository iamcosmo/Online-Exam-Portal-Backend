using Domain.Models;
using Infrastructure.DTOs.adminDTOs;
using Infrastructure.DTOs.ExamDTOs;
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

        Task<List<Exam>> ExamsToBeApprovedList();
        Task<int> ApproveExamAsync(ExamApprovalStatusDTO dto);
        //Task<bool> ReviewReportedQuestionAsync(int questionId);

        List<QuestionReport> GetAllReportedQuestionsAsync();
        QuestionReport? GetReportedQuestionByIdAsync(int qid);
        Task<bool> UpdateReportedQuestionStatusAsync(int qid, int status);

        Task<int> BlockUserAsync(int userId);
        Task<IEnumerable<ExamFeedbackViewDTO>> GetExamFeedbacksAsync(int examId);

        //Task<List<Question>> GetAllReportedQuestionsAsync();

        Task<int> AddAdminRemarks(int examId, string remarks);
        Task<List<ApproveTopicsDTO>> TopicsToBeApprovedAsync(int userId);

        Task<int> ApproveOrRejectTopic(int topicId, int userId);

    }

}

