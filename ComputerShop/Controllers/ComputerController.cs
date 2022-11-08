using ComputerShop.Models.MediatR.Commands;
using ComputerShop.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ComputerShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComputerController : ControllerBase
    {
        private readonly IMediator mediator;

        public ComputerController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [AllowAnonymous]
        [HttpGet(nameof(GetAll))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var computers = await mediator.Send(new GetAllComputersCommand());
            if (computers.Count() <= 0)
            {
                return NotFound("There aren't any computers in the collection");
            }
            return Ok(computers);
        }

        [HttpPost(nameof(Add))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
        public async Task<IActionResult> Add([FromBody]ComputerRequest computer)
        {
            var result = await mediator.Send(new AddComputerCommand(computer));

            if (result.HttpStatusCode == HttpStatusCode.BadRequest)
                return BadRequest(result);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet(nameof(GetById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Id must be greater than 0");
            var result = await mediator.Send(new GetByIdComputerCommand(id));

            if (result == null) return NotFound(id);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet(nameof(GetByName))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByName(string name)
        {
            if (name.Length <= 0 || string.IsNullOrEmpty(name)) return BadRequest("Name can't be null");
            var result = await mediator.Send(new GetByNameComputerCommand(name));

            if (result == null) return NotFound(name);

            return Ok(result);
        }

        [HttpPut(nameof(Update))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
        public async Task<IActionResult> Update([FromBody]ComputerRequest computer)
        {
            if (computer == null) return BadRequest("Computer can't be null");

            var result = await mediator.Send(new UpdateComputerCommand(computer));

            if (result.HttpStatusCode == HttpStatusCode.NotFound)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete(nameof(Delete))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("Id must be greater than 0");

            var result = await mediator.Send(new DeleteComputerCommand(id));

            if (result.HttpStatusCode == HttpStatusCode.NotFound)
                return NotFound(id);

            return Ok(result);
        }
    }
}