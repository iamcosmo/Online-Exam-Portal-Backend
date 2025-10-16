using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.TopicDTOs
{
    public class CreateTopicBodyDTO
    {
        public string TopicName { get; set; } = "";
        public int examinerId { get; set; } = 0;
    }
}
