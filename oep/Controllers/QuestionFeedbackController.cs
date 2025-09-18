using Domain.Models;
using Infrastructure.DTOs.QuestionFeedbackDTO;
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

        [Authorize(Roles = "Student")]
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

        [Authorize(Roles = "Admin,Examiner")]
        [HttpGet("get-question-feedback-by-QID(e-a)/{qId}")]
        public async Task<IActionResult> GetQuestionFeedback(int qId)
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token claims.");
            }

            var result = await _questionFeedbackRepository.GetFeedbackByQuestionId(qId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Examiner")]
        [HttpGet("get-all-question-feedbacks(e-a)")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            
            var result = await _questionFeedbackRepository.GetAllFeedbacks();
            return Ok(result);
        }

        [Authorize(Roles = "Student")]
        [HttpGet("update-question-feedback/{qId}")]
        public async Task<IActionResult> UpdateYourFeedback([FromBody] int uerId, [FromBody] string newFeedback, int qId)
        {
            var result = await _questionFeedbackRepository.UpdateQuestionFeedback(newFeedback, qId, uerId);
            if (result > 0)            
                return Ok("Feedback Updated Successfully");            
            else            
                return Ok("No such Feedback Found!");            
        }



    }
}
