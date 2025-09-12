using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Infrastructure.Repositories.Implementations
{
    public class ExamRepository : IExamRepository
    {
        private readonly AppDbContext _context;

        public ExamRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<int> AddExam(Exam exam)
        {
            await _context.Exams.AddAsync(exam);
            return await _context.SaveChangesAsync();

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

        public List<Exam> GetExamsAttemptedByUser(int UserId)
        {
            var user = _context.Users
                   .Include(u => u.ExamUsers)
                   .FirstOrDefault(u => u.UserId == UserId);

            return (List<Exam>)user.ExamUsers;

        }
        public int GetExamAttempts(int userId, int examId)
        {
            var result = _context.Results.FirstOrDefault(r => r.UserId == userId && r.Eid == examId);
            return (int)result.Attempts;
        }

        public StartExamResponseDTO StartExam(int examId)
        {
            var Data = _context.Exams.Include(e => e.Questions)
                .Where(e => e.Eid == examId)
                .Select(e => new StartExamResponseDTO
                {
                    EID = e.Eid,
                    TotalMarks = e.TotalMarks,
                    Duration = e.Duration,
                    Name = e.Name,
                    DisplayedQuestions = e.DisplayedQuestions,
                    Questions = e.Questions.Select(
                            q => new StartExamQuestionsDTO
                            {
                                Qid = q.Qid,
                                Type = q.Type,
                                QuestionName = q.Question1,
                                Marks = q.Marks,
                                Options = q.Options,
                                ApprovalStatus = q.ApprovalStatus
                            }
                        ).ToList()
                }).ToList()
                .FirstOrDefault();

            return Data;
        }
    }
}


