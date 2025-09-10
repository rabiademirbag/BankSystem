using BankSystem.Business.DataProtection;
using BankSystem.Business.Operations.Security.Dtos;
using BankSystem.Business.Types;
using BankSystem.Data.Entities;
using BankSystem.Data.Enums;
using BankSystem.Data.Repositories;
using BankSystem.Data.UnitOfWork;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.Security
{
    public class SecurityManager : ISecurityService
    {
        private readonly IRepository<SecurityEntity> _securityRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDataProtection _dataProtection;
        public SecurityManager(IRepository<SecurityEntity> securityRepository, IUnitOfWork unitOfWork, IRepository<UserEntity> userRepository, IDataProtection dataProtection)
        {
            _securityRepository = securityRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _dataProtection = dataProtection;
        }

        public async Task<ServiceMessage> ChangePassword(int userId, ChangePasswordDto passwordDto,string ipAddress)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var currentPassword = _dataProtection.UnProtect( user.Password);
            if (currentPassword != passwordDto.Password) 
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Girilen şifre hatalı"
                };
            }
            user.Password = _dataProtection.Protect( passwordDto.NewPassword);
            await _userRepository.UpdateAsync(user);
            await _securityRepository.AddAsync(new SecurityEntity
            {
                UserId = userId,
                ActionDate = DateTime.UtcNow,
                ActionType = SecurityActionType.PasswordChange,
                IpAddress = ipAddress
            });
            await _unitOfWork.SaveChangesAsync();

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Şifre başarıyla değiştirildi"
            };
            }

        public async Task<List<SecurityEntity>> GetLogs(int userId)
        {
            return await _securityRepository.GetAll(x => x.UserId == userId).ToListAsync();
        }

        public async Task<ServiceMessage> SetTwoFactorAuth(int userId, TwoFactorAuthDto twoFactorAuthDto, string ipAddress)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if(user is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Kullanıcı bulunamadı"
                };
            }
            user.TwoFactorEnabled = twoFactorAuthDto.Enabled;
            await _userRepository.UpdateAsync(user);
            
            var actionType = twoFactorAuthDto.Enabled ? SecurityActionType.TwoFactorEnabled : SecurityActionType.TwoFactorVerified;

            await _securityRepository.AddAsync(new SecurityEntity
            {
                UserId = userId,
                ActionType = actionType,
                IpAddress = ipAddress,
                ActionDate = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync();
            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "2FA ayarı başarıyla güncellendi"
            };

        }
    }
}
