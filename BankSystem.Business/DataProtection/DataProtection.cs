using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.DataProtection
{
    public class DataProtection : IDataProtection
    {
        private readonly IDataProtector _protector;

        public DataProtection(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("BankSystem-security-v1");
        }
        public string Protect(string password)
        {
            return _protector.Protect(password);
        }

        public string UnProtect(string protectedPassword)
        {
            return _protector.Unprotect(protectedPassword);
        }
    }
}
