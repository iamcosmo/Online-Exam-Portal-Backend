using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Analytics
{
    public class StudentAnalyticsDTO
    {
        public required int UserId { get; set; }
        
        public required List<AverageScoreMultipleAttempts> AverageScoreMultipleAttempts { get; set; }

        public int TotalExamsTaken { get; set; }

        public int TotalQuestionsEncountered { get; set; }

        public AttemptsRecords ExamAttemptsRecords { get; set; }

        public required List<TopicWiseAverageScore> OverallAverageScoreTopicWise { get; set; }

    }

    public class AverageScoreMultipleAttempts
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public double AverageScore { get; set; }

        public int TotalAttempts { get; set; }
    }

    public class AttemptsRecords
    {
        public int SingleAttempts { get; set; }
        public int DoubleAttempts{ get; set; }
        public int TrippleAttempts { get; set; }
    }

    public class TopicWiseAverageScore
    {
        public int TopicId { get; set; }
        public string Topic { get; set; }
        public double AverageScore { get; set; }
    }


}
