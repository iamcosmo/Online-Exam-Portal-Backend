using Domain.Data;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class ExamRepository : IExamRepository
    {
        private readonly AppDbContext _context;

        public ExamRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public int AddExam(Exam exam)
        {
            _context.Exams.Add(exam);
            return _context.SaveChanges();

        }

        public int UpdateExam(Exam exam)
        {
            Exam ToBeUpdatedExam = _context.Exams.FirstOrDefault(e => e.Eid == exam.Eid);
            if (ToBeUpdatedExam.TotalMarks != exam.TotalMarks)
            {
                ToBeUpdatedExam.TotalMarks = exam.TotalMarks;
            }
            else if (ToBeUpdatedExam.TotalQuestions != exam.TotalQuestions)
            {
                ToBeUpdatedExam.TotalQuestions = exam.TotalQuestions;
            }
            return _context.SaveChanges();
        }
        public List<Exam> GetExams()
        {
            return _context.Exams.ToList();
        }

        public Exam GetExamById(int examId)
        {
            return _context.Exams.FirstOrDefault(e => e.Eid == examId);
        }
    }
}


