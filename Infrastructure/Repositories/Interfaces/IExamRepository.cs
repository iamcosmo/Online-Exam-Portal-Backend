using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IExamRepository
    {
        public int AddExam(Exam exam);
        public int UpdateExam(Exam exam);
        public List<Exam> GetExams();
        public Exam GetExamById(int examId);
    }
}
