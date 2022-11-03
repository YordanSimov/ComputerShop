using ComputerShop.Models.MediatR.Commands.BrandCommands;
using ComputerShop.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ComputerShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IMediator mediator;

        public BrandController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet(nameof(GetAll))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var brands = await mediator.Send(new GetAllBrandsCommand());
            if (brands.Count() <= 0)
            {
                return NotFound("There aren't any brands in the collection");
            }
            return Ok(brands);
        }

        [HttpPost(nameof(Add))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Add([FromBody] BrandRequest brand)
        {
            var result = await mediator.Send(new AddBrandCommand(brand));

            if (result.HttpStatusCode == HttpStatusCode.BadRequest)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet(nameof(GetById))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Id must be greater than 0");
            var result = await mediator.Send(new GetByIdBrandCommand(id));

            if (result == null) return NotFound(id);

            return Ok(result);
        }

        [HttpGet(nameof(GetByName))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByName(string name)
        {
            if (name.Length <= 0 || string.IsNullOrEmpty(name)) return BadRequest("Name can't be null");
            var result = await mediator.Send(new GetByNameBrandCommand(name));

            if (result == null) return NotFound(name);

            return Ok(result);
        }

        [HttpPut(nameof(Update))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] BrandRequest brand)
        {
            if (brand == null) return BadRequest("Brand can't be null");

            var result = await mediator.Send(new UpdateBrandCommand(brand));

            if (result.HttpStatusCode == HttpStatusCode.NotFound)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete(nameof(Delete))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("Id must be greater than 0");

            var result = await mediator.Send(new DeleteBrandCommand(id));

            if (result.HttpStatusCode == HttpStatusCode.NotFound)
                return NotFound(id);

            return Ok(result);
        }
    }
}
