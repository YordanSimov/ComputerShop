using ComputerShop.Models.Configurations;
using ComputerShop.Models.MediatR.Commands;
using ComputerShop.Models.MediatR.Commands.PurchaseCommands;
using ComputerShop.Models.MediatR.Commands.UserCommands;
using ComputerShop.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> BuyComputer(int computerId)
        {
            var computerCheck = await mediator.Send(new GetByIdComputerCommand(computerId));
            if (computerCheck == null)
            {
                return BadRequest("Computer does not exist");
            }

            var userId = this.User.FindFirst(IdentitySettings.UserId);
            var purchaseRequest = new PurchaseRequest()
            {
                ComputerId = computerId,
                UserId = Guid.Parse(userId.Value)
            };

            var purchase = await mediator.Send(new AddPurchaseCommand(purchaseRequest));

            return Ok(purchase);
        }

        [HttpGet(nameof(GetMyPurchases))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> GetMyPurchases()
        {
            var userId = this.User.FindFirst(IdentitySettings.UserId);

            var purchases = await mediator.Send(new GetUserPurchasesCommand(Guid.Parse(userId.Value)));
            if (purchases.Count() <= 0)
            {
                return NotFound("You haven't made any purchases yet");
            }
            return Ok(purchases);
        }

        [HttpGet(nameof(GetUserPurchases))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = IdentitySettings.Admin)]

        public async Task<IActionResult> GetUserPurchases(Guid userId)
        {
            var purchases = await mediator.Send(new GetUserPurchasesCommand(userId));
            if (purchases.Count() <= 0)
            {
                return NotFound("This user hasn't made any purchases yet");
            }
            return Ok(purchases);
        }

        [HttpGet(nameof(GetAllUsers))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = IdentitySettings.Admin)]

        public async Task<IActionResult> GetAllUsers()
        {
            var users = await mediator.Send(new GetAllUsersCommand());
            if (users.Count() <= 0)
            {
                return NotFound("There aren't any users registered.");
            }
            return Ok(users);
        }
    }
}
