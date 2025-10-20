using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs.adminDTOs;
using Infrastructure.DTOs.ExamDTOs;
using Infrastructure.DTOs.QuestionFeedbackDTO;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OEP.Controllers
{

    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]

    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }


        [HttpGet("approve-exam-list")]
        public async Task<IActionResult> ToBeApprovedExamListAction([FromQuery] int userid)
        {
            var examList = await _adminRepository.ExamsToBeApprovedList(userid);
            if (examList == null || !examList.Any())
            {
                return Ok(new { message = "No Exams to be Approved.", ExamList = new List<Exam>() });
            }
            else
            {
                return Ok(new { ExamList = examList });
            }
        }


        // Replace the ApproveOrRejectExam method with the following:

        // POST /approve-exam/{eid}/{action}
        [HttpPost("approve-exam")]
        //[HttpPost("approve-exam/{eid}")]
        public async Task<IActionResult> ApproveOrRejectExam([FromBody] ExamApprovalStatusDTO dto)
        {

            if (dto == null || string.IsNullOrWhiteSpace(dto.action) || (dto.action != "approve" && dto.action != "reject"))
            {
                return BadRequest("Invalid request data.");
            }
            dto.action = dto.action.ToLower();

            int result = await _adminRepository.ApproveExamAsync(dto);

            if (result >= 1)
            {
                return Ok($"Exam with ID {dto.eid} has been {(dto.action == "approve" ? "approved" : "rejected")}.");
            }
            else
            {
                return StatusCode(500, "Failed to update exam status.");
            }

        }




        [HttpGet("reported-questions")]
        public async Task<IActionResult> GetAllReportedQuestions([FromQuery] int adminId)
        {
            var questions = await _adminRepository.GetAllReportedQuestionsAsync(adminId);

            if (questions == null || !questions.Any())
            {
                return Ok(new { msg = "No reported questions found." });
            }

            return Ok(questions);
        }

        [HttpGet("review-questions/{qid}")]
        public async Task<IActionResult> GetReportedQuestionById(int qid)
        {
            var question = await _adminRepository.GetReportedQuestionByIdAsync(qid);
            if (question == null)
                return Ok(new { msg = "No reported questions found." });
            return Ok(question);
        }

        // POST /admin/review-questions/{qid}/{status}
        [HttpPost("review-questions")]
        public async Task<IActionResult> ReviewReportedQuestion([FromBody] QuestionReviewDTO dto)
        {
            if (dto.status != 0 && dto.status != 1)
                return BadRequest("Invalid status. Use 0 or 1.");

            var result = await _adminRepository.UpdateReportedQuestionStatusAsync(dto);

            if (!result)
                return Ok("No reported questions found.");

            return Ok($"Question with ID {dto.qid} has been {(dto.status == 1 ? "approved" : "rejected")}.");
        }


        [HttpPost("block-users")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> BlockUser([FromBody] BlockUserDTO dto)
        {
            int result = await _adminRepository.BlockUserAsync(dto.uid);
            if (result == -1)
            {
                return Unauthorized("You are not allowed to Block Admins.");
            }

            return result >= 1 ? Ok($"User with ID {dto.uid} blocked.") : BadRequest("Blocking failed.");
        }


        // GET /exam-feedback-review/{eid}
        [HttpGet("exam-feedback-review/{eid}")]
        public async Task<IActionResult> GetExamFeedbacks([FromRoute] int eid)
        {
            var feedbacks = await _adminRepository.GetExamFeedbacksAsync(eid);
            return feedbacks != null ? Ok(feedbacks) : NotFound("No feedbacks found.");
        }


        [HttpPost("add-adminremarks/{examId}")]
        public async Task<IActionResult> AddAdminRemarksAction(int examId, [FromBody] string remarks)
        {
            var status = await _adminRepository.AddAdminRemarks(examId, remarks);
            return status > 0 ? Ok("Admin Remarks added") : BadRequest("Some error occured while adding remarks");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("topic-list")]
        public async Task<IActionResult> TopicsToBeApprovedAction([FromQuery] int userId)
        {
            var topics = await _adminRepository.TopicsToBeApprovedAsync(userId);
            if (topics == null)
                return Ok(new { Message = "No topics Found For approval" });

            return Ok(new { Topics = topics });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("approve-topic")]
        public async Task<IActionResult> ApproveOrRejectTopicAction([FromBody] TopicActionDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Action))
            {
                return BadRequest(new { Message = "Invalid request data." });
            }
            var status = await _adminRepository.ApproveOrRejectTopic(dto.TopicId, dto.UserId, dto.Action);
            if (status == 1)
                return Ok(new { Message = $"Topic {dto.Action}d Successfully." });

            return NotFound(new { Message = "Topic not found or update failed." });
        }
    }





}
