using Domain.Data;
using Infrastructure.DTOs.ExamDTOs;
using Infrastructure.DTOs.ResultDTOs;
using Infrastructure.Repositories.Interfaces;
// using Microsoft.Data.SqlClient;
using Npgsql;

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

        public async Task<List<ExamSummaryDTO>> GetAllResultsForUser(int userid)
        {

            var appearedExamIds = await _context.Responses
            .Where(r => r.UserId == userid)
            .Select(r => r.Eid)
            .Distinct()
            .ToListAsync();

            List<ExamSummaryDTO> examSummaries = await _context.Exams
            .Where(e => appearedExamIds.Contains(e.Eid))
            .Select(exam => new ExamSummaryDTO
            {
                Eid = exam.Eid,
                ExamName = exam.Name ?? string.Empty,
                TotalMarks = exam.TotalMarks ?? 0M,

                // Fetch the AttemptsData for this specific Exam and User.
                // This is optional (can be an empty list if no results exist).
                AttemptsData = exam.Results
                    .Where(r => r.UserId == userid)
                    .Select(r => new AttemptDTO
                    {
                        Attempt = r.Attempts ?? 0,
                        Score = r.Score ?? 0M,
                        TakenOn = r.CreatedAt ?? DateTime.MinValue
                    })
                    .OrderBy(a => a.Attempt)
                    .ToList()
            })
            .ToListAsync();

            return examSummaries;
        }
        //Using ADO.NET
        public async Task<CreateResultDTO> CreateExamResults(int examid, int userid)
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                using (var command = new NpgsqlCommand("SELECT createexamresult(@ExamId, @UserId)", connection))
                {
                    command.Parameters.AddWithValue("@ExamId", examid);
                    command.Parameters.AddWithValue("@UserId", userid);
                
                    await connection.OpenAsync();
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                
                    string msg = rowsAffected > 0 ? "Result created" : "Result cannot be created";
                    return new CreateResultDTO(rowsAffected, msg);
                }

            }
            catch (PostgresException ex)
            {

                _logger.LogInformation("SQL Error: " + ex.Message);
                return new CreateResultDTO(-1, ex.Message); ;
            }

        }

        public async Task<ResultCalculationResponseDTO> ExecuteAndGetAllResultsAsync(int examId, int userId)
        {
            bool hasFreshSubmissions = false;
            try
            {
                // Step 1: Check if there are any fresh submissions BEFORE running the procedure.
                // This tells us if a new result is expected to be created.
                hasFreshSubmissions = await _context.Responses
                    .AnyAsync(r => r.Eid == examId && r.UserId == userId && r.IsSubmittedFresh == true);

                if (hasFreshSubmissions)
                {
                    // Step 2: Execute the stored procedure to calculate the new result.
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"SELECT createexamresult({examId}, {userId})"
                    );

                }

                // Step 3: ALWAYS fetch the complete history of results for the user and exam.
                var allResults = await _context.Results
                    .AsNoTracking()
                    .Where(r => r.Eid == examId && r.UserId == userId)
                    .OrderBy(r => r.Attempts)
                    .Select(group => new ResultDTO
                    {
                        attempt = group.Attempts,
                        score = group.Score,
                        takenOn = group.UpdatedAt
                    })
                    .ToListAsync();

                if (!allResults.Any())
                {
                    // If after all that, there are still no results, return a clear message.
                    return new ResultCalculationResponseDTO { Success = false, Message = "No results found for this exam." };
                }

                var examData = await _context.Exams.Where(e => e.Eid == examId)
                    .Select(e => new { e.Name, e.TotalMarks })
                    .FirstOrDefaultAsync();

                return new ResultCalculationResponseDTO
                {
                    Success = true,
                    Message = "Successfully retrieved results.",
                    NewResultCalculated = hasFreshSubmissions,
                    Eid = examId,
                    ExamName = examData.Name,
                    TotalMarks = examData.TotalMarks,
                    Results = allResults
                };
            }
            catch (PostgresException ex)
            {
                if (ex.Message.Contains("Maximum number of attempts reached."))
                {
                    return new ResultCalculationResponseDTO { Success = false, Message = ex.Message };
                }
                // Log the full exception (ex)
                return new ResultCalculationResponseDTO { Success = false, Message = "A database error occurred." };
            }
            catch (Exception ex)
            {
                // Log the full exception (ex)
                return new ResultCalculationResponseDTO { Success = false, Message = "An internal error occurred." };
            }
        }
    }
}
