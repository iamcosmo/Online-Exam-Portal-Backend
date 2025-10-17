using Infrastructure.DTOs.ExamDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IResultRespository
    {
        public Task<List<ExamResultsDTO>> ViewExamResults(int examid, int userid);
        public Task<int> CreateExamResults(int examid, int userid);

        public Task<List<ExamResultsDTO>> GetAllResultsForUser(int userid);
    }
}
