using Domain.Models;
using Infrastructure.DTOs.TopicDTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetTopicsAction()
        {

            List<Topic> topics = _topicRepo.GetTopics();
            if (topics == null) return Ok("No Topics Exists");
            return Ok(topics);
        }


        [HttpGet("get-topic/{topicId}")]
        public IActionResult GetTopicsAction([FromRoute] int topicId)
        {

            Topic topic = _topicRepo.GetTopics(topicId);
            if (topic == null) return Ok("No Topic Exists");
            return Ok(topic);
        }


        [HttpPost("add-topic")]
        public IActionResult CreateTopicAction(string TopicName)
        {

            var CreatedTopic = _topicRepo.CreateTopic(TopicName);
            if (CreatedTopic == null) return StatusCode(500, "Could not add Topic");

            return Ok(new { Message = "Topic Created", CreatedTopicStatus = CreatedTopic });
        }

        [HttpPost("update-topic/{Tid}")]
        public IActionResult UpdateTopicAction([FromBody] UpdateTopicDTO updateTopicdto, [FromRoute] int Tid)
        {
            var UpdatedTopic = _topicRepo.UpdateTopic(updateTopicdto.Name, Tid);
            if (UpdatedTopic == null) return StatusCode(500, "Could not update Topic");

            return Ok(new { Message = "Topic Updated", UpdatedTopicStatus = UpdatedTopic });
        }

        [HttpPost("delete-topic/{topicId}")]
        public IActionResult DeleteTopicAction([FromRoute] int topicId)
        {
            var DeletedTopic = _topicRepo.DeleteTopic(topicId);
            if (DeletedTopic == null) return StatusCode(500, "Could not delete Topic");

            return Ok(new { Message = "Topic Deleted", DeletedTopicStatus = DeletedTopic });
        }



    }
}
