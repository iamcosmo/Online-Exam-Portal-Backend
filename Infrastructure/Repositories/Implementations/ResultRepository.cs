using Domain.Data;
using Infrastructure.DTOs.ExamDTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ResultRepository> _logger;

        public ResultRepository(AppDbContext dbContext, IConfiguration configuration, ILogger<ResultRepository> logger)
        {
            _context = dbContext;
            _config = configuration;
            _logger = logger;
        }
        public async Task<List<ExamResultsDTO>> ViewExamResults(int examid, int userid)
        {
            List<ExamResultsDTO> results = await _context.Results.Where(r => r.Eid == examid && r.UserId == userid).Select(r => new ExamResultsDTO
            {
                UserId = r.UserId,
                Eid = r.Eid,
                Attempts = r.Attempts,
                Score = r.Score
            }).ToListAsync();

            return results;
        }

        public async Task<List<ExamResultsDTO>> GetAllResultsForUser(int userid)
        {
            List<ExamResultsDTO> results = await _context.Results.Where(r => r.UserId == userid).Select(r => new ExamResultsDTO
            {
                UserId = r.UserId,
                Eid = r.Eid,
                Attempts = r.Attempts,
                Score = r.Score
            }).ToListAsync();
            return results;
        }
        //Using ADO.NET
        public async Task<int> CreateExamResults(int examid, int userid)
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("CreateExamResult", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ExamId", examid);
                    command.Parameters.AddWithValue("@UserId", userid);

                    await connection.OpenAsync();
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0 ? 1 : 0;
                }
            }
            catch (SqlException ex)
            {

                _logger.LogInformation("SQL Error: " + ex.Message);
                return -1;
            }

        }
    }
}
