using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class GetExamDataDTO
    {
        public int eid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal totalMarks { get; set; }
        public decimal duration { get; set; }
        public string Tids { get; set; }
        public int displayedQuestions { get; set; }
        public int AttemptNo { get; set; }
    }
}
