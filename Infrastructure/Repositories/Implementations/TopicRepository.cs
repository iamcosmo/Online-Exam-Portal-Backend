using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        public List<Topic> GetTopicsForQuestions(int examId)
        {
            var exam = _context.Exams.FirstOrDefault(e => e.Eid == examId && e.SubmittedForApproval == false);
            if (exam == null)
            {
                return new List<Topic> { };
            }
            Console.WriteLine(exam.Tids);

            var topicIds = JsonConvert.DeserializeObject<List<int>>(exam.Tids);

            // Fetch topics from the database
            var topics = _context.Topics
                .Where(t => topicIds.Contains(t.Tid))
                .ToList();

            return topics;

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
        public int UpdateTopic(string TopicName, int Tid)
        {
            var topicToUpdate = _context.Topics.Find(Tid);
            if (topicToUpdate == null)
            {
                return 0;
            }

            topicToUpdate.Subject = TopicName;
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
