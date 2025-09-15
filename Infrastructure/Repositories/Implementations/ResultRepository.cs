using Domain.Data;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class ResultRepository : IResultRespository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public ResultRepository(AppDbContext dbContext, IConfiguration configuration)
        {
            _context = dbContext;
            _config = configuration;
        }
        public List<ExamResultsDTO> ViewExamResults(int examid, int userid)
        {
            List<ExamResultsDTO> results = _context.Results.Where(r => r.Eid == examid && r.UserId == userid).Select(r => new ExamResultsDTO
            {
                UserId = r.UserId,
                Eid = r.Eid,
                Attempts = r.Attempts,
                Score = r.Score
            }).ToList();

            return results;
        }
        //Using ADO.NET
        public int CreateExamResults(int examid, int userid)
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("CreateExamResult", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExamId", examid);
                    command.Parameters.AddWithValue("@UserId", userid);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0 ? 1 : 0;
                }
            }
        }
    }
}
