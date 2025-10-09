using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.ExamDTOs
{
    public class UpdateExamDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? TotalQuestions { get; set; }
        public decimal? Duration { get; set; }
        public List<string?>? Tids { get; set; }
        public int? DisplayedQuestions { get; set; }
        public int MarksPerQuestion { get; set; }
    }
}
