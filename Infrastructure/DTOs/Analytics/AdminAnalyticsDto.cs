using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Analytics
{
    public class AdminAnalyticsDto
    {
        public int TotalExams { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalStudents { get; set; }
        public int TotalExaminers { get; set; }
        public int BlockedExaminers { get; set; }
        public List<TopExamDto> TopExams { get; set; }
    }
}
