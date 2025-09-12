using Domain.Models;
using Infrastructure.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IExamRepository
    {
        public Task<int> AddExam(Exam exam);
        public int UpdateExam(Exam exam);
        public List<Exam> GetExams();
        public Exam GetExamById(int examId);

        // To-do GetExamsAttemptedByUser(id);
        // To-do GetExamAttempts(userId, examId)


    }
}
