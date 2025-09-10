using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Analytic
{
    public int Eid { get; set; }

    public int Sid { get; set; }

    public string UserId { get; set; } = null!;

    public decimal? Score { get; set; }

    public decimal? TotalMarks { get; set; }

    public string? Level { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Exam EidNavigation { get; set; } = null!;

    public virtual Subject SidNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
