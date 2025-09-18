using Domain.Models;
using Infrastructure.DTOs.ExamDTOs;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IExamRepository
    {
        public Task<int> AddExam(AddExamDTO dto);
        public int UpdateExam(int examId, UpdateExamDTO dto);
        public List<GetExamDataDTO> GetExams();
        public StudentExamViewDTO GetExams(int examId);
        public List<Exam> GetExamsForExaminer(int userid);
        public int SubmitExamForApproval(int examId);

        public Exam GetExamByIdForExaminer(int examId);
        public int DeleteExam(int examId);
        public List<Exam> GetExamsAttemptedByUser(int UserId);
        public int GetExamAttempts(int userId, int examId);
        public StartExamResponseDTO StartExam(int examId);
        public int SubmitExam(SubmittedExamDTO submittedData);
    }
}
