using ComputerShop.Models.Requests;
using ComputerShop.Models.Responses;
using MediatR;

namespace ComputerShop.Models.MediatR.Commands.ReportCommands
{
    public record CreateReportByTimeCommand(ReportRequest Report) : IRequest<ReportResponse>
    {
    }
}