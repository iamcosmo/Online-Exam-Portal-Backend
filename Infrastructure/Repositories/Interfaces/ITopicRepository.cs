using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ITopicRepository
    {
        public List<Topic> GetTopics();
        public Topic GetTopics(int topicId);
        public int CreateTopic(string TopicName);
        public int UpdateTopic(Topic topic);
        public int DeleteTopic(int topicId);
    }
}
