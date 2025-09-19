using Domain.Models;
using Infrastructure.DTOs.ExamDTOs;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IExamRepository
    {
        public Task<int> AddExam(AddExamDTO dto);
        public Task<int> UpdateExam(int examId, UpdateExamDTO dto);
        public Task<List<GetExamDataDTO>> GetExams();
        public Task<StudentExamViewDTO> GetExams(int examId);
        public Task<List<Exam>> GetExamsForExaminer(int userid);
        public Task<int> SubmitExamForApproval(int examId);

        public Task<Exam> GetExamByIdForExaminer(int examId);
        public Task<int> DeleteExam(int examId);
        public Task<List<Exam>> GetExamsAttemptedByUser(int UserId);
        public Task<int> GetExamAttempts(int userId, int examId);
        public Task<StartExamResponseDTO> StartExam(int examId);
        public Task<int> SubmitExam(SubmittedExamDTO submittedData);
    }
}
