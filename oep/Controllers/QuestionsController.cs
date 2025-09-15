using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OEP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : Controller
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionsController(IQuestionRepository repo)
        {
            _questionRepository = repo;
        }

        [HttpGet("questions-index")]
        public IActionResult Index()
        {
            return Ok("Index Page for Exam Controller");
        }

        [Authorize(Roles = "Examiner")]
        [HttpPost("add-question")]
        public async Task<IActionResult> AddQuestion([FromBody] QuestionDTO question, [FromQuery] int examId)
        {
          
            Question quest = new()
            {
                Type = question.type,
                Question1 = question.question,
                Marks = question.marks,
                Options = question.options,
                CorrectOptions = question.correctOptions,
                ApprovalStatus = question.ApprovalStatus
            };
            var result = await _questionRepository.AddQuestion(quest, examId);
            return result > 0 ? Ok("Question added successfully") : BadRequest("Failed to add Question");
        }


        [Authorize(Roles = "Examiner")]
        [HttpGet("get-question-by-id/{Id}")]
        public IActionResult GetQuestionById(int Id)
        {
            var result = _questionRepository.GetQuestionById(Id);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Failed to get Question");
            }
        }

        [Authorize(Roles = "Examiner")]
        [HttpGet("get-question-by-examId/{examId}")]
        public async Task<IActionResult> GetQuestionByExam(int examId)
        {
            var result = await _questionRepository.GetQuestionsByExamId(examId);
            if (result == null)
            {
                return StatusCode(500, "An error occurred while retrieving questions.");
            }
            if (result.Count == 0)
            {
                return NotFound("No questions found for the specified exam.");
            }
            return Ok(result);
        }

        [Authorize(Roles = "Examiner")]
        [HttpPut("update-question/{qId}")]
        public async Task<IActionResult> UpdateOneQuestion([FromBody] QuestionDTO question, int qId)
        {
            Question quest = new()
            {
                Type = question.type,
                Question1 = question.question,
                Marks = question.marks,
                Options = question.options,
                CorrectOptions = question.correctOptions,
                ApprovalStatus = question.ApprovalStatus
            };
            var result = await _questionRepository.UpdateQuestion(quest, qId);
            return result > 0 ? Ok("Question updated successfully") : NotFound("Question not found or no changes made");
        }
    }
}
