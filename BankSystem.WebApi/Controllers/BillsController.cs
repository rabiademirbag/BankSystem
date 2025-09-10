using BankSystem.Business.Operations.Bill.Dtos;
using BankSystem.Business.Operations.Transaction;
using BankSystem.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        private readonly IBillService _billService;

        public BillsController(IBillService billService)
        {
            _billService = billService;   
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> PayBill(int id)
        {
            var userId = int.Parse(User.FindFirst("Id").Value);
            var result = await _billService.PayBill(id,userId);
            if (result.IsSucceed)
                return Ok(result.Message);
            else
                return NotFound(result.Message);
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Add(AddBillRequest request)
        {
            var addBillDto = new AddBillDto
            {
                BillType = request.BillType,
                BillNo = request.BillNo,
                Amount = request.Amount,
                DueDate = request.DueDate,
                IsPaid = request.IsPaid,
                UserId = request.UserId,
            };
            var result = await _billService.Add(addBillDto);
            if (result.IsSucceed)
                return Ok(result.Message);
            else
                return BadRequest(result.Message);  
        }
    }
}
