using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Response
{
    public int Eid { get; set; }

    public int Qid { get; set; }

    public int UserId { get; set; }

    public string? Resp { get; set; }

    public decimal? RespScore { get; set; }

    public bool? IsSubmittedFresh { get; set; }

    public virtual Exam EidNavigation { get; set; } = null!;

    public virtual Question QidNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
