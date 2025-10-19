using Domain.Models;
using Infrastructure.DTOs.TopicDTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OEP.Controllers
{
    [Authorize(Roles = "Examiner")]
    [ApiController]
    [Route("api/[controller]")]
    public class TopicsController : Controller
    {
        private readonly ITopicRepository _topicRepo;
        public TopicsController(ITopicRepository topic)
        {
            _topicRepo = topic;
        }

        [HttpGet("get-topic")]
        public async Task<IActionResult> GetTopicsAction()
        {

            List<GetTopicsDTO> topics = await _topicRepo.GetTopics();
            if (topics == null) return Ok(new { msg = "No Topics Exists" });
            return Ok(topics);
        }

        [HttpGet("get-examiner-topic/{examinerId}")]
        public async Task<IActionResult> GetExaminerTopicsAction([FromRoute] int examinerId)
        {

            List<GetTopicsDTO> topics = await _topicRepo.GetExaminerTopics(examinerId);
            if (topics == null) return Ok(new { msg = "No Topics Exists" });
            return Ok(topics);
        }


        [HttpGet("get-topic/{topicId}")]
        public async Task<IActionResult> GetTopicsAction([FromRoute] int topicId)
        {

            Topic topic = await _topicRepo.GetTopics(topicId);
            if (topic == null) return Ok("No Topic Exists");
            return Ok(topic);
        }

        [HttpGet("get-exam-topics/{examId}")]
        public IActionResult GetExamTopicsAction([FromRoute] int examId)
        {
            var topics = _topicRepo.GetTopicsForQuestions(examId);
            return topics != null ? Ok(topics) : StatusCode(500, "Server not able to fetch related topics or no topics added.");

        }

        [HttpPost("add-topic")]
        public IActionResult CreateTopicAction([FromBody] CreateTopicBodyDTO createTopicdto)
        {

            var CreatedTopic = _topicRepo.CreateTopic(createTopicdto.TopicName, createTopicdto.examinerId);
            if (CreatedTopic == null) return StatusCode(500, "Could not add Topic");

            return Ok(new { Message = "Topic Created", TopicStatus = CreatedTopic });
        }

        [HttpPost("update-topic/{Tid}")]
        public IActionResult UpdateTopicAction([FromBody] UpdateTopicDTO updateTopicdto, [FromRoute] int Tid)
        {
            var UpdatedTopic = _topicRepo.UpdateTopic(updateTopicdto.Name, Tid);
            if (UpdatedTopic == null) return StatusCode(500, "Could not update Topic");

            return Ok(new { Message = "Topic Updated", TopicStatus = UpdatedTopic });
        }

        [HttpDelete("delete-topic/{topicId}")]
        public IActionResult DeleteTopicAction([FromRoute] int topicId)
        {
            var DeletedTopic = _topicRepo.DeleteTopic(topicId);
            if (DeletedTopic == null) return StatusCode(500, "Could not delete Topic");

            return Ok(new { Message = "Topic Deleted", TopicStatus = DeletedTopic });
        }

        [HttpPost("send-topic-for-approval/{topicId}")]
        public async Task<IActionResult> SendTopicForApproval([FromRoute] int topicId)
        {
            int status = await _topicRepo.SubmitTopicForApproval(topicId);
            return status > 0 ? Ok(new { Message = "Topic submitted for approval.", TopicStatus = status }) : BadRequest(new { Message = "Could not submit Topic for Approval." });

        }


    }
}
