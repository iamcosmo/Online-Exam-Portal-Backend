using Domain.Models;
using Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAllUsers();
        User? GetUserById(int id);
        List<User> GetUsersByRole(string role);
        int UpdateUser(int id, UpdateUserDTO dto);
        int DeleteUser(int id);

    }
}
