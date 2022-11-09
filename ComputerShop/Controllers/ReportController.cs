using ComputerShop.Models.Configurations;
using ComputerShop.Models.MediatR.Commands.ReportCommands;
using ComputerShop.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        [HttpPost(nameof(CreateReportByTime))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = IdentitySettings.Admin)]

        public async Task<IActionResult> CreateReportByTime(ReportRequest reportRequest)
        {
            var report = await mediator.Send(new CreateReportByTimeCommand(reportRequest));

            if (report.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(report.Message);
            }
            return Ok(report);
        }

        [HttpGet(nameof(GetAllReports))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = IdentitySettings.Admin)]
        public async Task<IActionResult> GetAllReports(DateTime? time)
        {
            var reports = await mediator.Send(new GetAllReportsCommand(time));
            if (reports.Count() <= 0)
            {
                return NotFound("There aren't any reports for this time");
            }
            return Ok(reports);
        }
    }
}
