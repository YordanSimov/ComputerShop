using ComputerShop.DL.MongoRepositories;
using ComputerShop.Models.MediatR.Commands.ReportCommands;
using ComputerShop.Models.Models;
using MediatR;

namespace ComputerShop.BL.CommandHandlers.ReportCommandHandler
{
    public class GetAllReportsCommandHandler : IRequestHandler<GetAllReportsCommand, IEnumerable<Report>>
    {
        private readonly IReportRepository reportRepository;

        public GetAllReportsCommandHandler(IReportRepository reportRepository)
        {
            this.reportRepository = reportRepository;
        }

        public async Task<IEnumerable<Report>> Handle(GetAllReportsCommand request, CancellationToken cancellationToken)
        {
            return await reportRepository.GetAllReports(request.Time);
        }
    }
}
