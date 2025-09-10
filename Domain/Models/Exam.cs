using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Exam
{
    public int Eid { get; set; }

    public int? Sid { get; set; }

    public int? TotalQuestions { get; set; }

    public decimal? TotalMarks { get; set; }

    public decimal? Duration { get; set; }

    public string? Description { get; set; }

    public string? Name { get; set; }

    public int? ApprovalStatus { get; set; }

    public string? ApprovedByUserId { get; set; }

    public int? DisplayedQuestions { get; set; }

    public virtual ICollection<Analytic> Analytics { get; set; } = new List<Analytic>();

    public virtual User? ApprovedByUser { get; set; }

    public virtual ICollection<ExamFeedback> ExamFeedbacks { get; set; } = new List<ExamFeedback>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();

    public virtual Subject? SidNavigation { get; set; }
}
