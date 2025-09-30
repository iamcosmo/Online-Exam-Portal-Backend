using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Analytics
{
    public class TopExamDto
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public double AverageScore { get; set; }
    }
}
