using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Password { get; set; }

    public DateOnly? Dob { get; set; }

    public string? PhoneNo { get; set; }

    public DateOnly? RegistrationDate { get; set; }

    public string? Role { get; set; }

    public bool? IsBlocked { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Analytic> Analytics { get; set; } = new List<Analytic>();

    public virtual ICollection<ExamFeedback> ExamFeedbacks { get; set; } = new List<ExamFeedback>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<QuestionReport> QuestionReports { get; set; } = new List<QuestionReport>();

    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
