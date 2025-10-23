using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.ResultDTOs
{
    public class CreateResultDTO
    {
        public CreateResultDTO(int st, string m) { status = st; msg = m; }
        public int status { get; set; }
        public string msg { get; set; }
    }
    public class ResultDTO
    {
        public int? attempt { get; set; }
        public decimal? score { get; set; }
        public DateTime? takenOn { get; set; }
    }
    public class ResultCalculationResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool NewResultCalculated { get; set; }
        public int Eid { get; set; }
        public string? ExamName { get; set; }
        public decimal? TotalMarks { get; set; }
        public List<ResultDTO>? Results { get; set; }
    }
}
