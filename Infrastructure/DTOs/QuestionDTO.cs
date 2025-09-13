using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class QuestionDTO
    {
        public int eid { get; set; }

        public string type { get; set; }

        public string question { get; set; }

        public decimal marks { get; set; }

        public string options { get; set; }

        public string correctOptions { get; set; }


    }
}
