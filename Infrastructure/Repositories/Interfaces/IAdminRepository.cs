using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Infrastructure.Repositories.Interfaces
{

        public interface IAdminRepository
        {
            Task<bool> ApproveExamAsync(int examId,int status);
            //Task<bool> ReviewReportedQuestionAsync(int questionId);
            Task<bool> BlockUserAsync(int userId);
            Task<IEnumerable<ExamFeedback>> GetExamFeedbacksAsync(int examId);

             //Task<List<Question>> GetAllReportedQuestionsAsync();

           Task<List<QuestionReport>> GetAllReportedQuestionsAsync();
           Task<QuestionReport?> GetReportedQuestionByIdAsync(int qid);
          Task<bool> UpdateReportedQuestionStatusAsync(int qid, int status);

    }

}

