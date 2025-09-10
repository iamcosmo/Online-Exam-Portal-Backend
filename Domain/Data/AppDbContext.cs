using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Analytic> Analytics { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamFeedback> ExamFeedbacks { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionReport> QuestionReports { get; set; }

    public virtual DbSet<Response> Responses { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LTIN487764;User ID=sa;Password=password-1;Initial Catalog=OEP_DB;Encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Analytic>(entity =>
        {
            entity.HasKey(e => new { e.Eid, e.Sid, e.UserId }).HasName("PK__Analytic__70260A50F35BE1CC");

            entity.Property(e => e.Eid).HasColumnName("EID");
            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Level)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Score).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalMarks)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Total_Marks");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.EidNavigation).WithMany(p => p.Analytics)
                .HasForeignKey(d => d.Eid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Analytics__EID__5DCAEF64");

            entity.HasOne(d => d.SidNavigation).WithMany(p => p.Analytics)
                .HasForeignKey(d => d.Sid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Analytics__SID__5EBF139D");

            entity.HasOne(d => d.User).WithMany(p => p.Analytics)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Analytics__UserI__5FB337D6");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Eid).HasName("PK__Exam__C190170B43619CA5");

            entity.ToTable("Exam");

            entity.Property(e => e.Eid)
                .ValueGeneratedNever()
                .HasColumnName("EID");
            entity.Property(e => e.ApprovedByUserId)
                .HasMaxLength(255)
                .HasColumnName("ApprovedByUserID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Duration).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.TotalMarks).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.Exams)
                .HasForeignKey(d => d.ApprovedByUserId)
                .HasConstraintName("FK__Exam__ApprovedBy__4316F928");

            entity.HasOne(d => d.SidNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.Sid)
                .HasConstraintName("FK__Exam__SID__4222D4EF");
        });

        modelBuilder.Entity<ExamFeedback>(entity =>
        {
            entity.HasKey(e => new { e.Eid, e.UserId }).HasName("PK__Exam_Fee__10E89BC1F3ABA591");

            entity.ToTable("Exam_Feedback");

            entity.Property(e => e.Eid).HasColumnName("EID");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("UserID");
            entity.Property(e => e.Feedback).IsUnicode(false);

            entity.HasOne(d => d.EidNavigation).WithMany(p => p.ExamFeedbacks)
                .HasForeignKey(d => d.Eid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exam_Feedba__EID__5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.ExamFeedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exam_Feed__UserI__59063A47");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Qid).HasName("PK__Question__CAB147CB732E22A2");

            entity.ToTable("Question");

            entity.Property(e => e.Qid)
                .ValueGeneratedNever()
                .HasColumnName("QID");
            entity.Property(e => e.Eid).HasColumnName("EID");
            entity.Property(e => e.Marks).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Question1)
                .HasColumnType("text")
                .HasColumnName("Question");
            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.EidNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.Eid)
                .HasConstraintName("FK__Question__EID__46E78A0C");

            entity.HasOne(d => d.SidNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.Sid)
                .HasConstraintName("FK__Question__SID__45F365D3");
        });

        modelBuilder.Entity<QuestionReport>(entity =>
        {
            entity.HasKey(e => new { e.Qid, e.UserId }).HasName("PK__Question__1BC9CB01584FABE5");

            entity.ToTable("QuestionReport");

            entity.Property(e => e.Qid).HasColumnName("QID");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("UserID");

            entity.HasOne(d => d.QidNavigation).WithMany(p => p.QuestionReports)
                .HasForeignKey(d => d.Qid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QuestionRep__QID__5441852A");

            entity.HasOne(d => d.User).WithMany(p => p.QuestionReports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QuestionR__UserI__5535A963");
        });

        modelBuilder.Entity<Response>(entity =>
        {
            entity.HasKey(e => new { e.Eid, e.Qid, e.UserId }).HasName("PK__Response__C02C8BBBF4003A5C");

            entity.ToTable("Response");

            entity.Property(e => e.Eid).HasColumnName("EID");
            entity.Property(e => e.Qid).HasColumnName("QID");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("UserID");
            entity.Property(e => e.RespScore)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Resp_Score");

            entity.HasOne(d => d.EidNavigation).WithMany(p => p.Responses)
                .HasForeignKey(d => d.Eid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Response__EID__49C3F6B7");

            entity.HasOne(d => d.QidNavigation).WithMany(p => p.Responses)
                .HasForeignKey(d => d.Qid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Response__QID__4AB81AF0");

            entity.HasOne(d => d.User).WithMany(p => p.Responses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Response__UserID__4BAC3F29");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.Eid }).HasName("PK__Results__BB91CDDC4D199CBC");

            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("UserID");
            entity.Property(e => e.Eid).HasColumnName("EID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Score).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.EidNavigation).WithMany(p => p.Results)
                .HasForeignKey(d => d.Eid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Results__EID__5165187F");

            entity.HasOne(d => d.User).WithMany(p => p.Results)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Results__UserID__5070F446");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK__Subjects__CA195970F934229F");

            entity.Property(e => e.Sid)
                .ValueGeneratedNever()
                .HasColumnName("SID");
            entity.Property(e => e.ApprovedByUserId)
                .HasMaxLength(255)
                .HasColumnName("ApprovedByUserID");
            entity.Property(e => e.Subject1)
                .HasMaxLength(255)
                .HasColumnName("Subject");

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.ApprovedByUserId)
                .HasConstraintName("FK__Subjects__Approv__3F466844");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC9573E765");

            entity.ToTable("User");

            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNo).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
