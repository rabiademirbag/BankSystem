using BankSystem.Business.Operations.Security.Dtos;
using BankSystem.Business.Types;
using BankSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.Security
{
    public interface ISecurityService
    {
        Task<ServiceMessage> SetTwoFactorAuth(int userId, TwoFactorAuthDto twoFactorDto, string ipAddress);

        Task<ServiceMessage> ChangePassword(int userId, ChangePasswordDto passwordDto,string ipAddress);
        Task<List<SecurityEntity>> GetLogs(int userId);
    }
}
