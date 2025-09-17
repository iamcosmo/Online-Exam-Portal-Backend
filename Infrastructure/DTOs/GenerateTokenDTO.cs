using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class GenerateTokenDTO
    {
        public string role { get; set; }
        public string email { get; set; }
    }
}
