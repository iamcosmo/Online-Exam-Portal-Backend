using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.QuestionFeedbackDTO
{
    public class GetQuestionFeedback
    {
        public string feedback { get; set; }

        public int qId { get; set; }

        public int userId { get; set; }
    }
}
