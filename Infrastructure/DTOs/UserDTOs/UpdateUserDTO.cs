using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        public string? FullName { get; set; }
        public DateOnly? Dob { get; set; }
        public string? PhoneNo { get; set; }
    }
}
