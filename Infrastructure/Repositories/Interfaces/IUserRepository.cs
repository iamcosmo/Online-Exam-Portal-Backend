using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        int RegisterUser(User user);
        List<User> GetAllUsers();
        User? GetUserById(int id);
        List<User> GetUsersByRole(string role);
        int UpdateUser(User user);
        int DeleteUser(int id);

        User? Login(int userId, string password);


    }
}
