using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class AddQuestionFeedback
    {
        public int qid { get; set; }

        public string feedback { get; set; }

        public int userId { get; set; }
    }
}
