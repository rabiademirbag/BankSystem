using BankSystem.Business.Operations.Account;
using BankSystem.Business.Operations.Account.Dtos;
using BankSystem.Data.Enums;
using BankSystem.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace BankSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _accountService.GetAccounts();
            if (accounts is null)
                return NotFound("Hesap bulunamadı");
            else
                return Ok(accounts);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAcount(AddAccountRequest request)
        {
            var userId = int.Parse(User.FindFirst("Id").Value); 
            var addAccountDto = new AddAccountDto
            {
                AccountType = request.AccountType
            };
            var result = await _accountService.AddAccount(addAccountDto,userId);
            if (result.IsSucceed)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);
        }

        [HttpGet("{id}/balance")]
        public async Task<IActionResult> GetBalance(int id)
        {
            var balance = await _accountService.GetBalance(id);
            if (!balance.IsSucceed)
                return NotFound(balance.Message);
            else
                return Ok(balance.Data);
            
        }
        [HttpGet("{id}/transactions")]
        public async Task<IActionResult> GetTransactions(int id)
        {
            var transactions= await _accountService.GetTransactions(id);
            if(transactions is null)
                return NotFound("Hesap bulunamadı");
            else
                return Ok(transactions);
        }

        [HttpPost("{id}/transfer")]
        public async Task<IActionResult> MoneyTransfer(MoneyTransferRequest request,int id)
        {
            var moneyTransferDto = new MoneyTransferDto
            {
                Amount = request.Amount,
                ReceiverAccountNo = request.ReceiverAccountNo,
                ReceiverFullName = request.ReceiverFullName,
                Description = request.Description,

            };
           var result = await _accountService.MoneyTransfer(moneyTransferDto,id);
            if (result.IsSucceed)
            
                return Ok(result.Message);
            else
                return NotFound(result.Message);
        }

        [HttpPost("{id}/set-default")]
        public async Task<IActionResult> SetDefautl(int id)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);

            var result = await _accountService.SetDefaultAccount(id,userId);

            if (result.IsSucceed)
                return Ok(result.Message);
            else
                return NotFound(result.Message);
        }
    }
}
