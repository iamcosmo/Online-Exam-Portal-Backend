using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.QuestionsDTO
{
    public class ListQuestionsDTO
    {
        public string QuestionName { get; set; }

        public int QuestionId { get; set; }

        public string QuestionType { get; set; }
    }
}
