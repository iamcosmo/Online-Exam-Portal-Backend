using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Question
    {
        [Key]
        public required string QID { get; set; }

        public required string SID { get; set; }

        public required string EID { get; set; }

        public string Type { get; set; }

        public required string QuestionText { get; set; }

        public decimal Marks { get; set; }

        public required string Options { get; set; }

        public required int ApprovalStatus { get; set; }

    }
}
