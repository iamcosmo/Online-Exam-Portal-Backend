using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Topic
{
    public Topic() { ApprovalStatus = 0; }
    public int Tid { get; set; }

    public string? Subject { get; set; }

    public int? ApprovalStatus { get; private set; }

    public int? ApprovedByUserId { get; set; }

    public virtual User? ApprovedByUser { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
