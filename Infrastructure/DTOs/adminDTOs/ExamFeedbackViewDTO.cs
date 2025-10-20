using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.adminDTOs
{
    public class ExamFeedbackViewDTO
    {
        public int Eid { get; set; }
        public string Feedback { get; set; }
        public int StudentId { get; set; }

        public int ApprovalStatus { get; set; }
    }
}
