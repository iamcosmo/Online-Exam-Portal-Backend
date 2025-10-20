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

        public Task<string> AddQuestionFeedbackDTO(AddQuestionFeedbackDTO qFeedback);

        public Task<List<GetQuestionFeedback>> GetFeedbackByQuestionId(int qid);

        public Task<List<GetQuestionFeedback>> GetAllFeedbacks();

        public Task<List<GetQuestionFeedback>> GetAllFeedbacks(int userId);

        public Task<int> UpdateQuestionFeedback(string updatedFeedback, int qid, int uId);
    }
}
