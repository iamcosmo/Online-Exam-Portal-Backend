using Domain.Data;
using Infrastructure.DTOs.Analytics;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OEP.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsRepository analyticsRepository;

        public AnalyticsController(IAnalyticsRepository ar)
        {
            analyticsRepository = ar;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AdminAnalyticsDto>> GetAdminAnalyticsAction()
        {
            var analyticsData = await analyticsRepository.GetAdminAnalytics();
            if (analyticsData != null)
            {
                return Ok(analyticsData);
            }
            else return Ok("No data available");
        }

        [HttpGet("examiner/{examinerId}")]
        [Authorize(Roles = "Examiner")]
        public async Task<ActionResult<ExaminerAnalyticsDto>> GetExaminerAnalyticsAction(int examinerId)
        {
            var examinerAnalyticsData = await analyticsRepository.GetExaminerAnalytics(examinerId);

            if (examinerAnalyticsData != null) return Ok(examinerAnalyticsData);
            else return Ok("No Data available or Examiner Not Found");
        }

        //[HttpGet("student/{userId}")]
        //[Authorize(Roles = "Student")]

        


    }
}
