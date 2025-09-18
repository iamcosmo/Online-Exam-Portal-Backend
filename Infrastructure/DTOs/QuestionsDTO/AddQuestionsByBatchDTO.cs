using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.QuestionsDTO
{
    public class QuestionItemDTO // Changed from 'internal' to 'public' to fix CS0053
    {
        public string Type { get; set; }
        public string Question { get; set; }
        public decimal Marks { get; set; }
        public string Options { get; set; }
        public List<string> CorrectOptions { get; set; }
        public int ApprovalStatus { get; set; }
    }

    public class AddQuestionsByBatchDTO
    {
        public int Tid { get; set; }
        public required List<QuestionItemDTO> Questions { get; set; }
    }
}
