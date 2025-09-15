using Domain.Models;
using Infrastructure.DTOs;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IExamRepository
    {
        public Task<int> AddExam(Exam exam);
        public int UpdateExam(Exam exam);
        public List<GetExamDataDTO> GetExams();
        public StudentExamViewDTO GetExams(int examId);
        public List<Exam> GetExamsForExaminer(int userid);

        public Exam GetExamByIdForExaminer(int examId);
        public int DeleteExam(int examId);
        public List<Exam> GetExamsAttemptedByUser(int UserId);
        public int GetExamAttempts(int userId, int examId);
        public StartExamResponseDTO StartExam(int examId);
        public int SubmitExam(SubmittedExamDTO submittedData);
    }
}
