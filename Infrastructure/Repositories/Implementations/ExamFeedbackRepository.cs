using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations
{
 

        public class ExamFeedbackRepository : IExamFeedbackRepository
        {
            private readonly AppDbContext _context;

            public ExamFeedbackRepository(AppDbContext context)
            {
                _context = context;
            }

            public void AddFeedback(int examId, int userId, ExamFeedbackDto dto)
            {
                var feedback = new ExamFeedback
                {
                    Eid = examId,
                    UserId = userId,
                    Feedback = dto.Feedback
                };

                _context.ExamFeedbacks.Add(feedback);
                _context.SaveChanges();
            }

            public IEnumerable<ExamFeedback> GetAllFeedbacks(int examId)
            {
                return _context.ExamFeedbacks
                    .Include(f => f.User)
                    .Include(f => f.EidNavigation)
                    .Where(f => f.Eid == examId)
                    .ToList();
            }

            public IEnumerable<ExamFeedbackDto> GetStudentFeedback(int examId, int userId)
            {
                return _context.ExamFeedbacks
                    .Where(f => f.Eid == examId && f.UserId == userId)
                    .Select(f => new ExamFeedbackDto
                    {
                        Feedback = f.Feedback
                    })
                    .ToList();
            }
        }

    }

