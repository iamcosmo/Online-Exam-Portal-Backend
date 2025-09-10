using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Result
{
    public string UserId { get; set; } = null!;

    public int Eid { get; set; }

    public int? Attempts { get; set; }

    public decimal? Score { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Exam EidNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
