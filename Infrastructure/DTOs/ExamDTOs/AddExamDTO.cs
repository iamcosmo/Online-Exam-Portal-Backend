using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.ExamDTOs
{
    public class AddExamDTO
    {
        public string Name { get; set; }
        public int userId { get; set; }
        public string? Description { get; set; }
        public int TotalQuestions { get; set; }
        public decimal Duration { get; set; }
        public List<int> Tids { get; set; }
        public int? DisplayedQuestions { get; set; }
        public int MarksPerQ { get; set; }
    }

}
