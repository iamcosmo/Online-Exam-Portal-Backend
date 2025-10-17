using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Topic
{
    public Topic() { ApprovalStatus = 0; }
    public int Tid { get; set; }
    public int? ExaminerId { get; set; }

    public string? Subject { get; set; }

    public int? ApprovalStatus { get; private set; }

    public void SetApprovalStatus(int val) { this.ApprovalStatus = val; }

    public int? ApprovedByUserId { get; set; }
    public bool SubmittedForApproval { get; set; }

    public virtual User? ApprovedByUser { get; set; }
    public virtual User? Examiner { get; set; }
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
