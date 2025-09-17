using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.ExamDTOs
{
    public class StartExamQuestionsDTO
    {
        public int Qid { get; set; }

        public string? Type { get; set; }

        public string? QuestionName { get; set; }

        public decimal? Marks { get; set; }

        public string? Options { get; set; }

        public int? ApprovalStatus { get; set; }

        public virtual ICollection<QuestionReport> QuestionReports { get; set; } = new List<QuestionReport>();
    }
}
