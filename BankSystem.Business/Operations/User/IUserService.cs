using BankSystem.Business.Operations.User.Dtos;
using BankSystem.Business.Types;
using BankSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.User
{
    public interface IUserService
    {
        Task<ServiceMessage> Register(RegisterDto registerDto);
        Task<ServiceMessage<UserInfoDto>> Login(LoginDto loginDto);
        Task<UserInfoDto> GetUser(int id);

        Task<ServiceMessage<UserInfoDto>> UpdateUser(UpdateDto updateDto, int id);

        Task<ServiceMessage> Delete(int id);
        Task<ServiceMessage> GenerateTwoFactorCode(int userId);

        Task<ServiceMessage<UserEntity>> VerifyTwoFactorCode(int userId,string code);
    }
}
