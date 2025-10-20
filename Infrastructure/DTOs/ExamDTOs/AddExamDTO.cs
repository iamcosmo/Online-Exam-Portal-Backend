using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.ExamDTOs
{
    public class AddExamDTO
    {
        public string Name { get; set; }
        public int userId { get; set; }
        public string? Description { get; set; }
        public int TotalQuestions { get; set; }
        public decimal Duration { get; set; }
        public List<int> Tids { get; set; }
        public int? DisplayedQuestions { get; set; }
        public int MarksPerQuestion { get; set; }
    }

    public class AddExamResponseDTO
    {

        public AddExamResponseDTO(int status, int eid)
        {
            this.status = status;
            this.examId = eid;
        }

        public int status { get; set; }
        public int examId { get; set; }
    }

}
