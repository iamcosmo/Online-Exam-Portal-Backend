using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class ExamFeedback
{
    public int Eid { get; set; }

    public string? Feedback { get; set; }

    public int UserId { get; set; }

    public virtual Exam EidNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
