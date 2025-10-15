using Domain.Models;
using Infrastructure.DTOs;
using Infrastructure.DTOs.TopicDTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ITopicRepository
    {
        public Task<List<GetTopicsDTO>> GetTopics();
        public Task<Topic> GetTopics(int topicId);
        public List<Topic> GetTopicsForQuestions(int examId);
        public int CreateTopic(string TopicName);
        public int UpdateTopic(string TopicName, int Tid);
        public int DeleteTopic(int topicId);
        public Task<int> SubmitTopicForApproval(int topicId);
    }
}
