using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public partial class Exam
{
    public Exam()
    {
        ApprovalStatus = 0;
    }
    public int Eid { get; set; }

    [ForeignKey("User")]
    public int? UserId { get; set; }

    public string? Tids { get; set; }

    public int? TotalQuestions { get; set; }

    public decimal? TotalMarks { get; set; }

    public decimal? Duration { get; set; }

    public string? Description { get; set; }

    public string? Name { get; set; }

    public int? ApprovalStatus { get; set; }

    public void setApprovalStatus() { this.ApprovalStatus = this.ApprovalStatus == 0 ? 1 : 0; }

    public int? ApprovedByUserId { get; private set; }

    public int? DisplayedQuestions { get; set; }

    public string? AdminRemarks { get; set; }

    public virtual User? ApprovedByUser { get; set; }

    public virtual ICollection<ExamFeedback> ExamFeedbacks { get; set; } = new List<ExamFeedback>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();

    public virtual User? User { get; set; }
}
