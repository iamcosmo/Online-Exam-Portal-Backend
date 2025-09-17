using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.AuthDTOs
{
    public class RegistrationInputDTO
    {
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string Password { get; set; }
        public DateOnly? Dob { get; set; }
        public string? PhoneNo { get; set; }
        public string Token { get; set; }
    }
}
