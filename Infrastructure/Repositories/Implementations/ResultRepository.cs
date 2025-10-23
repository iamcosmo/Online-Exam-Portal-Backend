using Domain.Data;
using Infrastructure.DTOs.ExamDTOs;
using Infrastructure.DTOs.ResultDTOs;
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

        public async Task<List<ExamSummaryDTO>> GetAllResultsForUser(int userid)
        {
            List<ExamSummaryDTO> groupedResults = await _context.Results
            .Where(r => r.UserId == userid)
            .Include(r => r.EidNavigation)

            .GroupBy(r => new
            {
                r.Eid,
                r.EidNavigation.Name,
                r.EidNavigation.TotalMarks
            })
            .Select(group => new ExamSummaryDTO
            {
                Eid = group.Key.Eid,
                ExamName = group.Key.Name ?? string.Empty,


                TotalMarks = group.Key.TotalMarks ?? 0M,

                AttemptsData = group
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

            return groupedResults;
        }
        //Using ADO.NET
        public async Task<CreateResultDTO> CreateExamResults(int examid, int userid)
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
                    string msg = rowsAffected > 0 ? "Result created" : "Result cannot be created";
                    return new CreateResultDTO(rowsAffected, msg);
                }
            }
            catch (SqlException ex)
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
                        $"EXEC CreateExamResult @ExamId = {examId}, @UserId = {userId}"
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
            catch (Microsoft.Data.SqlClient.SqlException ex)
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
