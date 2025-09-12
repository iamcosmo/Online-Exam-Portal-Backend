using Domain.Models;
using Infrastructure.DTOs;
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

        public int DeleteExam(int examId);

        public List<Exam> GetExamsAttemptedByUser(int UserId);
        public int GetExamAttempts(int userId, int examId);
        public StartExamResponseDTO StartExam(int examId);


    }
}
