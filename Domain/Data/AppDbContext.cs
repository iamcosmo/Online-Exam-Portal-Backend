using System;
using System.Collections.Generic;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamFeedback> ExamFeedbacks { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionReport> QuestionReports { get; set; }

    public virtual DbSet<Response> Responses { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Validation> Validations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LTIN487764;User ID=sa;Password=password-1;Initial Catalog=OEP_DB;Encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Eid).HasName("PK__Exams__C190170BF0041EC8");

            entity.Property(e => e.Eid).HasColumnName("EID");
            entity.Property(e => e.ApprovedByUserId).HasColumnName("ApprovedByUserID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Duration).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Tids).HasColumnName("TIDs");
            entity.Property(e => e.TotalMarks).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.ExamApprovedByUsers)
                .HasForeignKey(d => d.ApprovedByUserId)
                .HasConstraintName("FK__Exams__ApprovedB__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.ExamUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Exams__UserId__619B8048");
        });

        modelBuilder.Entity<ExamFeedback>(entity =>
        {
            entity.HasKey(e => new { e.Eid, e.UserId }).HasName("PK__ExamFeed__10E89BCF1456D595");

            entity.Property(e => e.Eid).HasColumnName("EID");
            entity.Property(e => e.Feedback).IsUnicode(false);

            entity.HasOne(d => d.EidNavigation).WithMany(p => p.ExamFeedbacks)
                .HasForeignKey(d => d.Eid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExamFeedbac__EID__76969D2E");

            entity.HasOne(d => d.User).WithMany(p => p.ExamFeedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExamFeedb__UserI__778AC167");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Qid).HasName("PK__Question__CAB147CB6DDE6F19");

            entity.Property(e => e.Qid).HasColumnName("QID");
            entity.Property(e => e.Eid).HasColumnName("EID");
            entity.Property(e => e.Marks).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Question1)
                .HasColumnType("text")
                .HasColumnName("Question");
            entity.Property(e => e.Tid).HasColumnName("TID");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.EidNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.Eid)
                .HasConstraintName("FK__Questions__EID__656C112C");

            entity.HasOne(d => d.TidNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.Tid)
                .HasConstraintName("FK__Questions__TID__6477ECF3");
        });

        modelBuilder.Entity<QuestionReport>(entity =>
        {
            entity.HasKey(e => new { e.Qid, e.UserId }).HasName("PK__Question__1BC9CB0F94FD49DF");

            entity.Property(e => e.Qid).HasColumnName("QID");

            entity.HasOne(d => d.QidNavigation).WithMany(p => p.QuestionReports)
                .HasForeignKey(d => d.Qid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QuestionRep__QID__72C60C4A");

            entity.HasOne(d => d.User).WithMany(p => p.QuestionReports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QuestionR__UserI__73BA3083");
        });

        modelBuilder.Entity<Response>(entity =>
        {
            entity.HasKey(e => new { e.Eid, e.Qid, e.UserId }).HasName("PK__Response__202C8BBBC68F22B9");

            entity.Property(e => e.Eid).HasColumnName("EID");
            entity.Property(e => e.Qid).HasColumnName("QID");
            entity.Property(e => e.RespScore)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Resp_Score");

            entity.HasOne(d => d.EidNavigation).WithMany(p => p.Responses)
                .HasForeignKey(d => d.Eid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Responses__EID__68487DD7");

            entity.HasOne(d => d.QidNavigation).WithMany(p => p.Responses)
                .HasForeignKey(d => d.Qid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Responses__QID__693CA210");

            entity.HasOne(d => d.User).WithMany(p => p.Responses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Responses__UserI__6A30C649");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.Eid }).HasName("PK__Results__BB91CD3CE65C72A6");

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
                .HasConstraintName("FK__Results__EID__6FE99F9F");

            entity.HasOne(d => d.User).WithMany(p => p.Results)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Results__UserId__6EF57B66");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Tid).HasName("PK__Topics__C456D7299E012EDB");

            entity.Property(e => e.Tid).HasColumnName("TID");
            entity.Property(e => e.ApprovedByUserId).HasColumnName("ApprovedByUserID");
            entity.Property(e => e.Subject).HasMaxLength(255);

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.Topics)
                .HasForeignKey(d => d.ApprovedByUserId)
                .HasConstraintName("FK__Topics__Approved__5DCAEF64");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C58DFEAA5");

            entity.ToTable("User");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNo).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Validation>(entity =>
        {
            entity.HasNoKey();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
