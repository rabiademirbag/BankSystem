using BankSystem.Business.DataProtection;
using BankSystem.Business.Operations.Sms;
using BankSystem.Business.Operations.User.Dtos;
using BankSystem.Business.Types;
using BankSystem.Data.Entities;
using BankSystem.Data.Enums;
using BankSystem.Data.Repositories;
using BankSystem.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.User
{
    public class UserManager : IUserService
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDataProtection _dataProtection;
        private readonly IRepository<AccountEntity> _accountRepository;

        private readonly ISmsService _smsService;
        public UserManager(IRepository<UserEntity> userRepository, IUnitOfWork unitOfWork, IDataProtection dataProtection, IRepository<AccountEntity> accountRepository, ISmsService smsService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _dataProtection=dataProtection;
            _accountRepository = accountRepository;
            _smsService = smsService;
        }

        public async Task<ServiceMessage> Delete(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if(user is null)
            {
                return null;
            }
            await _userRepository.DeleteAsync(user);
            try
            {
                _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Kullanıcı silinirken bir hata oluştu");
            }
            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Kullanıcı başarıyla silindi"
            };
        }

        public async Task<UserInfoDto> GetUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if(user is null)
            {
                return null;
            }
            return new UserInfoDto
            {
                Id=user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserType = user.UserType
            };

        }
        

        public async Task<ServiceMessage<UserInfoDto>> Login(LoginDto loginDto)
        {
            var hasUser = await _userRepository.GetAsync(x => x.NationalId == loginDto.NationalId);
            if (hasUser is null)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = "Kullanıcı bulunamadı"
                };
            }
            var unprotectedPassword = _dataProtection.UnProtect(hasUser.Password);

            if (unprotectedPassword == loginDto.Password)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = true,
                    Data = new UserInfoDto
                    {
                        Id = hasUser.Id,
                        FirstName = hasUser.FirstName,
                        LastName = hasUser.LastName,
                        Email = hasUser.Email,
                        UserType = hasUser.UserType,
                    }
                };
            }
            return new ServiceMessage<UserInfoDto>
            {
                IsSucceed = false,
                Message = "Şifre ya da TC Kimlik no hatalı"
            };
        }
        
        
        public async Task<ServiceMessage> Register(RegisterDto registerDto)
        {
            var hasUser =  _userRepository.GetAll(x=>x.Email.ToLower()== registerDto.Email.ToLower());
            if(hasUser.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu email adresi zaten kayıtlı"
                };
            }
            var userEntity = new UserEntity
            {
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Password = _dataProtection.Protect(registerDto.Password),
                PhoneNumber = registerDto.PhoneNumber,
                NationalId = registerDto.NationalId,
                UserType=UserType.Customer

            };
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _userRepository.AddAsync(userEntity);
                await _unitOfWork.SaveChangesAsync();

                var accountEntity = new AccountEntity
                {
                    AccountNo = await GenerateAccountNumberAsync(), 
                    Balance = 0,
                    AccountType = AccountType.Checking,
                    UserId = userEntity.Id
                };

                await _accountRepository.AddAsync(accountEntity);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollBackAsync();
                throw new Exception("Kayıt sırasında bir hata oluştu");
            }

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Kullanıcı kaydı başarılı"
            };
        }

        private async Task<string> GenerateAccountNumberAsync()
        {
            string accountNo;
            do
            {
                accountNo = Guid.NewGuid().ToString("N").Substring(0, 10);
            } while ((_accountRepository.GetAll(a => a.AccountNo == accountNo)).Any());

            return accountNo;
        }
        public async Task<ServiceMessage<UserInfoDto>> UpdateUser(UpdateDto updateDto, int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if(user is null) 
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = false,
                    Message = "Kullanıcı bulunamadı"
                };
            }
            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.Email = updateDto.Email;
            user.PhoneNumber = updateDto.PhoneNumber;

            await _userRepository.UpdateAsync(user);

            try
            {
                _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new Exception("Güncelleme sırasında bir hata meydana geldi");
            }

            return new ServiceMessage<UserInfoDto>
            {
                Message = "Güncelleme başarılı",
                IsSucceed = true,
                Data = new UserInfoDto
                {
                    FirstName = updateDto.FirstName,
                    LastName = updateDto.LastName,
                    Email = updateDto.Email,
                    Id=user.Id,
                    UserType=user.UserType
                }
            };
        }

        public async Task<ServiceMessage> GenerateTwoFactorCode(int userId)
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
             var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            user.TwoFactorCode = code;
            user.TwoFactorExpireTime = DateTime.Now.AddMinutes(5);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            await _smsService.SendSmsAsync(user.PhoneNumber, $"Giriş doğrulama kodunuz: {code}");

            return new ServiceMessage
            {
                IsSucceed = true,
                Message = "Doğrulama kodu gönderildi"
            };

        }

        public async Task<ServiceMessage<UserEntity>> VerifyTwoFactorCode(int userId, string code)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                return new ServiceMessage<UserEntity>
                {
                    IsSucceed = false,
                    Message = "Kullanıcı bulunamadı"
                };
            }
            if(user.TwoFactorCode!=code || user.TwoFactorExpireTime < DateTime.Now)
            {
                return new ServiceMessage<UserEntity>
                {
                    IsSucceed = false,
                    Message = "Kod geçersiz veya süresi dolmuş"
                };
            }
            user.TwoFactorCode = null;
            user.TwoFactorExpireTime = null;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new ServiceMessage<UserEntity>
            {
                IsSucceed = true,
                Data = user,
                Message = "Doğrulama başarılı"
            };
        }
    }
}
