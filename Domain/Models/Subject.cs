using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Subject
{
    public int Sid { get; set; }

    public string? Subject1 { get; set; }

    public int? ApprovalStatus { get; set; }

    public string? ApprovedByUserId { get; set; }

    public virtual ICollection<Analytic> Analytics { get; set; } = new List<Analytic>();

    public virtual User? ApprovedByUser { get; set; }

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
