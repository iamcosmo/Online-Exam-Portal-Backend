using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.ExamDTOs
{
    public class ExamApprovalStatusDTO
    {
        public int ExamId { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
    }
}
