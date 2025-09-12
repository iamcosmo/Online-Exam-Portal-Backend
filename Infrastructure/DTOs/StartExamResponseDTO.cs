using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class StartExamResponseDTO
    {
        public int EID { get; set; }
        public decimal? TotalMarks { get; set; }

        public decimal? Duration { get; set; }
        public string? Name { get; set; }
        public int? DisplayedQuestions { get; set; }
        public virtual List<StartExamQuestionsDTO> Questions { get; set; } = new List<StartExamQuestionsDTO>();
    }
}
