using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DTOs.ExamDTOs;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IExamFeedbackRepository
    {
        public int AddFeedback(int examId, ExamFeedbackDto dto);
        public IEnumerable<ExamFeedback> GetAllFeedbacks(int examId);
        public IEnumerable<ExamFeedbackDto> GetStudentFeedback(int examId, int userId);
    }
}
