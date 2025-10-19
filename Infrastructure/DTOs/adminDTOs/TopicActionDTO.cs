using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.adminDTOs
{
    public class TopicActionDTO
    {
        public int TopicId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
    }
}
