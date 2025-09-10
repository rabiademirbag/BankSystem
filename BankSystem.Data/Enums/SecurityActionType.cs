using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Enums
{
    public enum SecurityActionType
    {
        Login = 1,
        Logout = 2,
        FailedLogin = 3,
        PasswordChange = 4,
        TwoFactorEnabled = 5,
        TwoFactorVerified = 6
    }
}
