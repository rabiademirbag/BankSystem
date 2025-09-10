using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Business.Operations.Sms
{
    public class SmsManager : ISmsService
    {
        public Task SendSmsAsync(string phoneNumber, string message)
        {
            Console.WriteLine($"[DUMMY SMS] To: {phoneNumber}, Message {message}");
            return Task.CompletedTask;
        }
    }
}
