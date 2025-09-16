using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OEP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionFeedbackController : Controller
    {
        private readonly IQuestionFeedbackRepository _questionFeedbackRepository;

        public QuestionFeedbackController(IQuestionFeedbackRepository questionFeedbackRepository)
        {
            _questionFeedbackRepository = questionFeedbackRepository;
        }
        [HttpGet("questions-feedback-index")]
        public IActionResult Index()
        {
            return Ok("QuestionFeedback Index Page");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-question-feedback")]
        public IActionResult AddQuestionFeedback([FromBody] AddQuestionFeedbackDTO qFeedback, [FromQuery] int qId)
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token claims.");
            }

            var userId = int.Parse(userIdClaim.Value);
            var feedback = new QuestionReport
            {
                Qid = qId,
                Feedback = qFeedback.feedback,
                UserId = userId
            };

            var result = _questionFeedbackRepository.AddQuestionFeedbackDTO(feedback);
            return result > 0 ? Ok("Feedback added successfully") : BadRequest("Failed to add feedback");
        }



    }
}
