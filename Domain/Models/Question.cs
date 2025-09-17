using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Question
{
    public int Qid { get; set; }

    public int? Tid { get; set; }

    public int? Eid { get; set; }

    public string? Type { get; set; }

    public string? Question1 { get; set; }

    public decimal? Marks { get; set; }

    public string? Options { get; set; }

    // JSON string like {"1":"Option A", "2":"Option B"}

    public string? CorrectOptions { get; set; }

    public int? ApprovalStatus { get; set; }

    public virtual Exam? EidNavigation { get; set; }

    public virtual ICollection<QuestionReport> QuestionReports { get; set; } = new List<QuestionReport>();

    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();

    public virtual Topic? TidNavigation { get; set; }
}
