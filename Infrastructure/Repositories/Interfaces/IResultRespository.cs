using Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IResultRespository
    {
        public List<ExamResultsDTO> ViewExamResults(int examid, int userid);
        public int CreateExamResults(int examid, int userid);
    }
}
