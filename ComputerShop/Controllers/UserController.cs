using ComputerShop.Models.MediatR.Commands;
using ComputerShop.Models.MediatR.Commands.PurchaseCommands;
using ComputerShop.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ComputerShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost(nameof(BuyComputer))]

        public async Task<IActionResult> BuyComputer([FromBody] PurchaseRequest purchaseRequest)
        {
            var computerCheck =await mediator.Send(new GetByIdComputerCommand(purchaseRequest.ComputerId));
            if (computerCheck == null)
            {
                return BadRequest("Computer does not exist");
            }

            var purchase = await mediator.Send(new AddPurchaseCommand(purchaseRequest));

            return Ok(purchase);
        }
    }
}
