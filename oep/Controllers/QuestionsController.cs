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
        public async Task<IActionResult> AddQuestion([FromBody] AddQuestionDTO question, [FromQuery] int examId)
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
        public async Task<IActionResult> UpdateOneQuestion([FromBody] UpdateQuestionDTO question, int qId)
        {
            var existingQuestion = _questionRepository.GetQuestionById(qId);
            if (existingQuestion == null)
            {
                return NotFound("Question not found");
            }

            bool isUpdated = false;

            if (question.type != null && question.type != existingQuestion.Type)
            {
                existingQuestion.Type = question.type;
                isUpdated = true;
            }
            if (question.question != null)
            {
                existingQuestion.Question1 = question.question;
                isUpdated = true;
            }
            if (question.marks.HasValue && question.marks != existingQuestion.Marks)
            {
                existingQuestion.Marks = question.marks;
                isUpdated = true;
            }
            if (question.options != null)
            {
                existingQuestion.Options = question.options;
                isUpdated = true;
            }
            if (question.correctOptions != null)
            {
                existingQuestion.CorrectOptions = question.correctOptions;
                isUpdated = true;
            }
            if (question.ApprovalStatus.HasValue && question.ApprovalStatus != existingQuestion.ApprovalStatus)
            {
                existingQuestion.ApprovalStatus = question.ApprovalStatus;
                isUpdated = true;
            }

            if (!isUpdated)
            {
                return Ok("No changes made to the question.");
            }

            var result = await _questionRepository.UpdateQuestion(existingQuestion, qId);
            return result > 0 ? Ok("Question updated successfully") : StatusCode(500, "Failed to update question");
        }
    }
}
