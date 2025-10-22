using Domain.Models;
using Infrastructure.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        StudentRegisterResponseDTO RegisterStudent(RegisterUserDTO dto);
        int RegisterAdminOrExaminer(RegistrationInputDTO examinerDTO, string role);
        User? Login(string email, string password);
        bool ValidateToken(string token);

        bool VerifyOTP(int userId, int otp);

        int ResendOTP(int userId);

    }
}
