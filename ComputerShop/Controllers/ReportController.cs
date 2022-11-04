using ComputerShop.Models.MediatR.Commands.ReportCommands;
using ComputerShop.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ComputerShop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IMediator mediator;

        public ReportController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]

        public async Task<IActionResult> CreateReportByTime(ReportRequest reportRequest)
        {
            var report = await mediator.Send(new CreateReportByTimeCommand(reportRequest));

            return Ok(report);
        }
    }
}
