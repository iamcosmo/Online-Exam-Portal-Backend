using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class ExamResultsDTO
    {
        public int UserId { get; set; }
        public int Eid { get; set; }
        public int? Attempts { get; set; }
        public decimal? Score { get; set; }
    }
}
