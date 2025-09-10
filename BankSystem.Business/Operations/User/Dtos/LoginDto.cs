using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.User.Dtos
{
    public class LoginDto
    {
        public string NationalId { get; set; }
        public string Password { get; set; }
    }
}
