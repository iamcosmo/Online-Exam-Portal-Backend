using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace OEP.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        // Replace the ApproveOrRejectExam method with the following:

        // POST /approve-exam/{eid}/{action}
        [HttpPost("approve-exam")]
        //[HttpPost("approve-exam/{eid}")]
        public async Task<IActionResult> ApproveOrRejectExam([FromBody] ExamApprovalStatusDTO dto)
        {

            //Console.WriteLine("eidvalue= " + eid + " action= " + action);

            //int status;

            if (dto == null || string.IsNullOrWhiteSpace(dto.action))
            {
                return BadRequest("Invalid request data.");
            }


            int status;
            string actionLower = dto.action.ToLower();

            if (actionLower == "approve")
            {
                status = 1;
            }
            else if (actionLower == "reject")
            {
                status = 0;
            }
            else
            {
                return BadRequest("Invalid action. Use 'approve' or 'reject'.");
            }


            bool result = await _adminRepository.ApproveExamAsync(dto.eid, status);

            if (result)
            {
                return Ok($"Exam with ID {dto.eid} has been {(status == 1 ? "approved" : "rejected")}.");
            }
            else
            {
                return StatusCode(500, "Failed to update exam status.");
            }

        }
      



        [HttpGet("reported-questions")]
        public async Task<IActionResult> GetAllReportedQuestions()
        {
            var questions = await _adminRepository.GetAllReportedQuestionsAsync();

            if (questions == null || !questions.Any())
            {
                return Ok("No reported questions found.");
            }

            return Ok(questions);
        }

        // GET /admin/review-questions/{qid}
        //Task<List<QuestionReport>> GetAllReportedQuestionsAsync();
        //Task<QuestionReport?> GetReportedQuestionByIdAsync(int qid);
        //Task<bool> UpdateReportedQuestionStatusAsync
      [HttpGet("review-questions/{qid}")]
        public async Task<IActionResult> GetReportedQuestionById(int qid)
        {
            var question = await _adminRepository.GetAllReportedQuestionsAsync();
            if (question == null)
                return Ok("No reported questions found.");
            return Ok(question);
        }

        // POST /admin/review-questions/{qid}/{status}
        [HttpPost("review-questions")]
        public async Task<IActionResult> ReviewReportedQuestion([FromBody] QuestionReviewDTO dto)
        {
            if (dto.status != 0 && dto.status != 1)
                return BadRequest("Invalid status. Use 0 or 1.");

            var result = await _adminRepository.UpdateReportedQuestionStatusAsync(dto.qid, dto.status);

            if (!result)
                return Ok("No reported questions found.");

            return Ok($"Question with ID {dto.qid} has been {(dto.status == 1 ? "approved" : "rejected")}.");
        }


        // oldPOST /review-questions/{qid}
        //[HttpPost("review-questions/{qid}")]
        //public async Task<IActionResult> ReviewReportedQuestion(int qid)
        //{
        //    var result = await _adminRepository.ReviewReportedQuestionAsync(qid);
        //    return result ? Ok("Question reviewed.") : BadRequest("Review failed.");
        //}

        // POST /block-users/{uid}
        [HttpPost("block-users")]
        public async Task<IActionResult> BlockUser([FromBody] BlockUserDTO dto)
        {
            var result = await _adminRepository.BlockUserAsync(dto.uid);

            return result ? Ok($"User with ID {dto.uid} blocked.") : BadRequest("Blocking failed.");
        }


        // GET /exam-feedback-review/{eid}
        [HttpGet("exam-feedback-review/{eid}")]
        public async Task<IActionResult> GetExamFeedbacks(int eid)
        {
            var feedbacks = await _adminRepository.GetExamFeedbacksAsync(eid);
            return feedbacks != null ? Ok(feedbacks) : NotFound("No feedbacks found.");
        }
    }





}
