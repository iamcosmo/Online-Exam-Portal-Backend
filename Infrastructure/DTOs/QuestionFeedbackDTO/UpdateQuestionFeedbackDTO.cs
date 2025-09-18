using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.QuestionFeedbackDTO
{
    public class UpdateQuestionFeedbackDTO
    {
        public string feedback { get; set; }

        public int userId { get; set; }
    }
}
