using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Analytics
{
    public class ExaminerAnalyticsDto
    {
        public int TotalExamsCreated { get; set; }
        public List<ExamScoreDto> AverageScoresPerExam { get; set; }
        public List<QuestionApprovalDto> QuestionApprovalStats { get; set; }
        public List<ExamParticipationDto> StudentParticipation { get; set; }
    }
    public class ExamScoreDto
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public double AverageScore { get; set; }
    }

    public class QuestionApprovalDto
    {
        public bool IsApproved { get; set; }
        public int Count { get; set; }
    }

    public class ExamParticipationDto
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int StudentCount { get; set; }
    }
}
