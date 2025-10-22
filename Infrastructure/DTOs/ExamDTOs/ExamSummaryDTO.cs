using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.ExamDTOs
{
    public class AttemptDTO
    {
        public int Attempt { get; set; }
        public decimal Score { get; set; }
        public DateTime TakenOn { get; set; }
    }
    public class ExamSummaryDTO
    {
        public int Eid { get; set; }
        public string ExamName { get; set; }
        public decimal TotalMarks { get; set; }

        public List<AttemptDTO> AttemptsData { get; set; }
    }
}
