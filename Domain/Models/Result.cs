using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public partial class Result
{
    [Key]
    public int Rid { get; set; }
    public int UserId { get; set; }

    public int Eid { get; set; }

    public int? Attempts { get; set; }

    public decimal? Score { get; set; }

    public DateTime? CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public virtual Exam EidNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
