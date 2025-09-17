using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class RegisterAdminExaminerDTO
    {
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string Password { get; set; }
        public DateOnly? Dob { get; set; }
        public string? PhoneNo { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
