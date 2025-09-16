using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        int RegisterUser(User user);
        User? Login(string email, string password);
    }
}
