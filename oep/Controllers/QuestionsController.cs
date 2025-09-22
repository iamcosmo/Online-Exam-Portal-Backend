using Domain.Models;
using Infrastructure.DTOs.ExamDTOs;
using Infrastructure.DTOs.QuestionsDTO;
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
        private readonly IExamRepository _examRepository;

        public QuestionsController(IQuestionRepository repo, IExamRepository examRepository)
        {
            _questionRepository = repo;
            _examRepository = examRepository;
        }

        [HttpGet("questions-index")]
        public IActionResult Index()
        {
            return Ok("Index Page for Exam Controller");
        }

        [Authorize(Roles = "Examiner")]
        [HttpPost("add-question")]
        public async Task<IActionResult> AddQuestion([FromBody] AddQuestionDTO question, [FromQuery] int examId, [FromQuery] int userId)
        {
            ExamWithQuestionsDTO exam = await _examRepository.GetExamByIdForExaminer(examId);
            if (exam == null)
                return BadRequest("Exam Not Found.");
            if (exam.UserId != userId)
                return Unauthorized("You are not authorized to modify this Exam!!");

            var tids_exams = exam.Tids ?? "[]";

            // Check if Tid is valid if Tids are present
            var tidsList = System.Text.Json.JsonSerializer.Deserialize<List<int>>(tids_exams);
            if (tidsList != null && tidsList.Count > 0 && !tidsList.Contains(question.Tid))
                return BadRequest("The provided Tid is not available for this exam.");

            var availableQuestionCount = await _questionRepository.GetQuestionsByExamId(examId);
            if (availableQuestionCount.Count + 1 > exam.TotalQuestions)
                return BadRequest("Adding this questions would exceed the total number of questions allowed for this exam.");

            var result = await _questionRepository.AddQuestion(question, examId);
            return result > 0 ? Ok("Question added successfully") : BadRequest("Failed to add Question");
        }

        [Authorize(Roles = "Examiner")]
        [HttpPost("add-questions-by-tid-batch")]

        public async Task<IActionResult> AddQuestionsByTidBatchToExam([FromBody] AddQuestionsByBatchDTO questions, [FromQuery] int examId, [FromQuery] int userId)
        {
            var exam = await _examRepository.GetExamByIdForExaminer(examId);

            if (exam == null)
                return BadRequest("Exam Not Found.");
            if (exam.UserId != userId)
                return Unauthorized("You are not authorized to modify this Exam!!");

            if (questions == null || questions.Questions.Count == 0)
                return BadRequest("Question list is empty.");    
           
            var availableQuestionCount = await _questionRepository.GetQuestionsByExamId(examId);
            if (availableQuestionCount.Count + questions.Questions.Count > exam.TotalQuestions)
                return BadRequest("Adding these questions would exceed the total number of questions allowed for this exam.");

            if (questions.Questions.Any(q => string.IsNullOrWhiteSpace(q.Question) || q.Marks <= 0))
                return BadRequest("One or more questions have invalid data.");

            var result = await _questionRepository.AddBatchQuestionsToExam(questions, examId);
            return result > 0 ? Ok("Questions added successfully") : BadRequest("Failed to add Questions");
        }


        [Authorize(Roles = "Examiner")]
        [HttpGet("get-question-by-id/{Id}")]
        public IActionResult GetQuestionById(int Id)
        {
            var result = _questionRepository.GetQuestionById(Id);
            if (result != null)
                return Ok(result);
            else
                return BadRequest("Failed to get Question");

        }

        [Authorize(Roles = "Examiner")]
        [HttpGet("get-question-by-examId/{examId}")]
        public async Task<IActionResult> GetQuestionByExam(int examId)
        {
            var result = await _questionRepository.GetQuestionsByExamId(examId);
            if (result == null)
                return StatusCode(500, "An error occurred while retrieving questions.");

            if (result.Count == 0)
                return NotFound("No questions found for the specified exam.");

            return Ok(result);
        }

        [Authorize(Roles = "Examiner")]
        [HttpPut("update-question/{qId}")]
        public async Task<IActionResult> UpdateOneQuestion([FromBody] UpdateQuestionDTO question, int qId)
        {
            var existingQuestion = _questionRepository.GetQuestionById(qId);
            if (existingQuestion == null)
                return NotFound("Question not found");

            var result = await _questionRepository.UpdateQuestion(question, qId);
            return result > 0 ? Ok("Question updated successfully") : StatusCode(500, "Failed to update question");
        }

        [Authorize(Roles = "Examiner")]
        [HttpDelete("delete-a-question/{qId}")]
        public async Task<IActionResult> DeleteOneQuestion(int qId)
        {
            var existingQuestion = _questionRepository.GetQuestionById(qId);
            if (existingQuestion == null)
                return NotFound("Question not found");

            var result = await _questionRepository.DeleteQuestion(qId);
            return result > 0 ? Ok("Question deleted successfully") : StatusCode(500, "Failed to delete question");
        }
    }
}
