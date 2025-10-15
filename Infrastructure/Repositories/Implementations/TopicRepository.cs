using Domain.Data;
using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.DTOs.TopicDTOs;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class TopicRepository : ITopicRepository
    {
        private readonly AppDbContext _context;
        public TopicRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<GetTopicsDTO>> GetTopics()
        {
            List<GetTopicsDTO> topicList = await _context.Topics
                .Select(t =>
                    new GetTopicsDTO
                    {
                        tid = t.Tid,
                        subject = t.Subject ?? "No Topic"
                    }).ToListAsync();

            return topicList;

        }
        public async Task<Topic> GetTopics(int topicId)
        {
            return await _context.Topics.FirstOrDefaultAsync(t => t.Tid == topicId);
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

        public async Task<int> SubmitTopicForApproval(int topicId)
        {
            var topic = await _context.Topics.FirstOrDefaultAsync(t => t.Tid == topicId);

            //Assigning a admin id
            var random = new Random();
            var adminIds = await _context.Users
                .Where(u => u.Role == "Admin")
                .Select(a => a.UserId)
                .ToListAsync();
            var randomAdminId = adminIds.OrderBy(x => random.Next()).FirstOrDefault();
            topic.ApprovedByUserId = randomAdminId;
            topic.SubmittedForApproval = true;

            return await _context.SaveChangesAsync();

        }
    }
}
