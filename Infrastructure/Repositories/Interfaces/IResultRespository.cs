using Infrastructure.DTOs.ExamDTOs;
using Infrastructure.DTOs.ResultDTOs;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IResultRespository
    {
        public Task<List<ExamResultsDTO>> ViewExamResults(int examid, int userid);
        public Task<CreateResultDTO> CreateExamResults(int examid, int userid);

        public Task<List<ExamResultsDTO>> GetAllResultsForUser(int userid);
    }
}
