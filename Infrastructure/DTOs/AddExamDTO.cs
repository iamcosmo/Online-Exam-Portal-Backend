using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class AddExamDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int TotalQuestions { get; set; }
        public decimal? TotalMarks { get; set; }
        public decimal Duration { get; set; }
        public string? Tids { get; set; }
        public int? DisplayedQuestions { get; set; }
    }

}
