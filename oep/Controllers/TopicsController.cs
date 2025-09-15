using Domain.Models;
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

        [HttpPost("get-topic")]
        public IActionResult GetTopicsAction()
        {

            List<Topic> topics = _topicRepo.GetTopics();
            if (topics == null) return Ok("No Topics Exists");
            return Ok(topics);
        }


        [HttpPost("get-topic/{topicId}")]
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

            return Ok(new { Message = "Topic Created", CreatedTopic = CreatedTopic });
        }

        [HttpPost("update-topic")]
        public IActionResult UpdateTopicAction(Topic topic)
        {
            var UpdatedTopic = _topicRepo.UpdateTopic(topic);
            if (UpdatedTopic == null) return StatusCode(500, "Could not update Topic");

            return Ok(new { Message = "Topic Updated", UpdatedTopic = UpdatedTopic });
        }

        [HttpPost("delete-topic/{topicId}")]
        public IActionResult DeleteTopicAction([FromRoute] int topicId)
        {
            var DeletedTopic = _topicRepo.DeleteTopic(topicId);
            if (DeletedTopic == null) return StatusCode(500, "Could not delete Topic");

            return Ok(new { Message = "Topic Deleted", DeletedTopic = DeletedTopic });
        }



    }
}
