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
        public int eid { get; set; }
        public string action { get; set; }
        public int userId { get; set; }
    }
}
