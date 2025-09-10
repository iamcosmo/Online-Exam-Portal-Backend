using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class ExamFeedback
{
    public int Eid { get; set; }

    public string? Feedback { get; set; }

    public string UserId { get; set; } = null!;

    public virtual Exam EidNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
