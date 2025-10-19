using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.ResultDTOs
{
    public class CreateResultDTO
    {
        public CreateResultDTO(int st, string m) { status = st; msg = m; }
        public int status { get; set; }
        public string msg { get; set; }
    }
}
