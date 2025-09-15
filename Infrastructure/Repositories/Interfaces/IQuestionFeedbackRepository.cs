using Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IQuestionFeedbackRepository
    {

        public int AddQuestionFeedbackDTO(AddQuestionFeedbackDTO qFeedback);

        public Task<string> GetFeedbackByQuestionId(int qid);

        public Task<int> UpdateQuestionFeedback(AddQuestionFeedbackDTO qFeedback, int qid);
    }
}
