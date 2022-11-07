using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.ReportCommands
{
    public record GetAllReportsCommand(DateTime Time) : IRequest<IEnumerable<Report>>
    {
    }
}
