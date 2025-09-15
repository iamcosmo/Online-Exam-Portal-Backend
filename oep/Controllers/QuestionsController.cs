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
          
            Question quest = new Question
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

        //[HttpPut("update-exam")]
        //public IActionResult UpdateExam([FromBody] Exam exam)
        //{
        //    var result = _examRepository.UpdateExam(exam);
        //    return result > 0 ? Ok("Exam updated successfully") : NotFound("Exam not found or no changes made");
        //}
    }
}
