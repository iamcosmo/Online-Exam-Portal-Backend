using System;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Domain.Data
{
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
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Try environment variable first (common in containers/hosts), then appsettings.json
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection")
                                       ?? configuration["ConnectionStrings__DefaultConnection"];

                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
                    {
                        npgsqlOptions.CommandTimeout(120);
                    });
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Minimal, explicit mapping where needed. Let EF infer defaults where possible.

            // Primary key for Validation (Token)
            modelBuilder.Entity<Validation>()
                .HasKey(v => v.Token);

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasKey(e => e.Eid);

                entity.Property(e => e.Eid).HasColumnName("EID");

                entity.Property(e => e.Name)
                      .HasMaxLength(255);

                // Description: keep as text (unlimited)
                entity.Property(e => e.Description)
                      .HasColumnType("text");

                // Numeric columns: use Postgres numeric
                entity.Property(e => e.Duration)
                      .HasColumnType("numeric(10,2)");

                entity.Property(e => e.TotalMarks)
                      .HasColumnType("numeric(10,2)");

                // MarksPerQuestion: let EF decide unless precision required
                // Boolean default
                entity.Property(e => e.SubmittedForApproval)
                      .HasDefaultValue(false);

                // Relations
                entity.HasOne(d => d.User)
                      .WithMany(p => p.ExamUsers)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Reviewer)
                      .WithMany(p => p.ExamReviewers)
                      .HasForeignKey(d => d.ReviewerId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ExamFeedback>(entity =>
            {
                entity.HasKey(e => new { e.Eid, e.UserId });

                entity.Property(e => e.Eid).HasColumnName("EID");
                entity.Property(e => e.Feedback).HasColumnType("text");

                entity.HasOne(d => d.EidNavigation)
                      .WithMany(p => p.ExamFeedbacks)
                      .HasForeignKey(d => d.Eid)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                      .WithMany(p => p.ExamFeedbacks)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.Qid);

                entity.Property(e => e.Qid).HasColumnName("QID");
                entity.Property(e => e.Eid).HasColumnName("EID");
                entity.Property(e => e.Tid).HasColumnName("TID");

                entity.Property(e => e.Question1)
                      .HasColumnName("Question")
                      .HasColumnType("text");

                entity.Property(e => e.Marks)
                      .HasColumnType("numeric(10,2)");

                entity.Property(e => e.Type)
                      .HasMaxLength(255);

                entity.HasOne(d => d.EidNavigation)
                      .WithMany(p => p.Questions)
                      .HasForeignKey(d => d.Eid)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.TidNavigation)
                      .WithMany(p => p.Questions)
                      .HasForeignKey(d => d.Tid)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<QuestionReport>(entity =>
            {
                entity.HasKey(e => new { e.Qid, e.UserId });

                entity.Property(e => e.Qid).HasColumnName("QID");

                entity.HasOne(d => d.QidNavigation)
                      .WithMany(p => p.QuestionReports)
                      .HasForeignKey(d => d.Qid)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                      .WithMany(p => p.QuestionReports)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Reviewer)
                      .WithMany(p => p.ReviewedQuestionReports)
                      .HasForeignKey(d => d.ReviewerId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Response>(entity =>
            {
                entity.HasKey(e => new { e.Eid, e.Qid, e.UserId });

                entity.Property(e => e.Eid).HasColumnName("EID");
                entity.Property(e => e.Qid).HasColumnName("QID");

                entity.Property(e => e.RespScore)
                      .HasColumnName("Resp_Score")
                      .HasColumnType("numeric(10,2)");

                entity.HasOne(d => d.EidNavigation)
                      .WithMany(p => p.Responses)
                      .HasForeignKey(d => d.Eid)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.QidNavigation)
                      .WithMany(p => p.Responses)
                      .HasForeignKey(d => d.Qid)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                      .WithMany(p => p.Responses)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(e => e.Rid);
                entity.Property(e => e.Rid).ValueGeneratedOnAdd();

                entity.Property(e => e.Eid).HasColumnName("EID");
                entity.Property(e => e.Score).HasColumnType("numeric(10,2)");

                // Timestamps: use Postgres now()
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp without time zone")
                      .HasDefaultValueSql("NOW()");

                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp without time zone")
                      .HasDefaultValueSql("NOW()");

                entity.HasOne(d => d.EidNavigation)
                      .WithMany(p => p.Results)
                      .HasForeignKey(d => d.Eid)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                      .WithMany(p => p.Results)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.HasKey(e => e.Tid);

                entity.Property(e => e.Tid).HasColumnName("TID");
                entity.Property(e => e.Subject).HasMaxLength(255);

                entity.HasOne(d => d.ApprovedByUser)
                      .WithMany(p => p.ApprovedTopics)
                      .HasForeignKey(d => d.ApprovedByUserId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Examiner)
                      .WithMany(p => p.CreatedTopics)
                      .HasForeignKey(d => d.ExaminerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("User");

                entity.Property(e => e.CreatedAt)
                      .HasColumnType("timestamp without time zone")
                      .HasDefaultValueSql("NOW()");

                entity.Property(e => e.UpdatedAt)
                      .HasColumnType("timestamp without time zone")
                      .HasDefaultValueSql("NOW()");

                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.FullName).HasMaxLength(255);
                entity.Property(e => e.Password).HasMaxLength(255);
                entity.Property(e => e.PhoneNo).HasMaxLength(255);
                entity.Property(e => e.Role).HasMaxLength(255);

                // Otp column kept as-is
                entity.Property(e => e.Otp).HasColumnName("Otp");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
