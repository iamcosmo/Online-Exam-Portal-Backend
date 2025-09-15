using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        public Task<int> AddQuestion(Question question, int eid);

        public Task<List<Question>> GetQuestionsByExamId(int examId);

        public Question GetQuestionById(int questionId);

        public Task<int> UpdateQuestion(Question question, int qid);

    }
}
