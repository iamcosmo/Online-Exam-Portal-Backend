using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.QuestionsDTO
{
    public class UpdateQuestionDTO
    {
        public string? type { get; set; }

        public string? question { get; set; }

        public string? options { get; set; }

        public List<string?> correctOptions { get; set; }

        public int? ApprovalStatus { get; set; }

    }
}
