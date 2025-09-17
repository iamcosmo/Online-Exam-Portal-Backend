using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace OEP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public MailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public IActionResult SendEmail(string toEmail,string subj,string messg)
        {
            _emailService.SendSimpleEmail(toEmail, subj, messg);
            return Ok("Email sent!");
        }

    }
}
