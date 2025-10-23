using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.AuthDTOs
{
    public class VerifyOtpRequestDTO
    {
        public int otp { get; set; }
        public bool AllowSuccessMail { get; set; }
    }
}
