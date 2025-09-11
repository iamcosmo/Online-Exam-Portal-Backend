using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Topic
{
    public int Tid { get; set; }

    public string? Subject { get; set; }

    public int? ApprovalStatus { get; set; }

    public int? ApprovedByUserId { get; set; }

    public virtual User? ApprovedByUser { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
