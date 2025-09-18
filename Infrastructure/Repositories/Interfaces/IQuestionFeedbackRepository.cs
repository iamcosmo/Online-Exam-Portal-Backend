using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.DTOs.QuestionFeedbackDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IQuestionFeedbackRepository
    {

        public int AddQuestionFeedbackDTO(QuestionReport qFeedback);

        public Task<string> GetFeedbackByQuestionId(int qid);

        public Task<List<GetQuestionFeedback>> GetAllFeedbacks();

        public Task<int> UpdateQuestionFeedback(string updatedFeedback, int qid, int uId);
    }
}
