using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.TopicDTOs
{
    public class GetTopicsDTO
    {
        public int? tid { get; set; }
        public string? subject { get; set; }
        public int? approvalStatus { get; set; }
        public bool? submittedForApproval { get; set; }
    }
}
