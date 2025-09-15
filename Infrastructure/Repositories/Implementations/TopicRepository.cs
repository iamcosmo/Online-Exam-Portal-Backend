using Domain.Data;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories.Implementations
{
    public class TopicRepository : ITopicRepository
    {
        private readonly AppDbContext _context;
        public TopicRepository(AppDbContext context)
        {
            _context = context;
        }
        public List<Topic> GetTopics()
        {
            return _context.Topics.ToList();
        }
        public Topic GetTopics(int topicId)
        {
            return (Topic)_context.Topics.Where(t => t.Tid == topicId);
        }
        public int CreateTopic(string TopicName)
        {
            Topic topic = new Topic
            {
                Subject = TopicName
            };
            var CreatedTopic = _context.Topics.Add(topic);
            return _context.SaveChanges();
        }
        public int UpdateTopic(Topic topic)
        {
            var CreatedTopic = _context.Topics.Update(topic);
            return _context.SaveChanges();
        }
        public int DeleteTopic(int topicId)
        {
            var topic = _context.Topics.Find(topicId);

            if (topic != null)
            {
                _context.Topics.Remove(topic);
                return _context.SaveChanges();
            }
            return 0;
        }
    }
}
