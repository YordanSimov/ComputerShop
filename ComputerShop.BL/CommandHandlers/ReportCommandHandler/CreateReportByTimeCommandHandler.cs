using AutoMapper;
using ComputerShop.DL.MongoRepositories;
using ComputerShop.Models.MediatR.Commands.ReportCommands;
using ComputerShop.Models.Models;
using ComputerShop.Models.Responses;
using MediatR;
using System.Net;

namespace ComputerShop.BL.CommandHandlers.ReportCommandHandler
{
    public class CreateReportByTimeCommandHandler : IRequestHandler<CreateReportByTimeCommand, ReportResponse>
    {
        private readonly IMapper mapper;
        private readonly IPurchaseRepository purchaseRepository;
        private readonly IReportRepository reportRepository;

        public CreateReportByTimeCommandHandler(IMapper mapper,IPurchaseRepository purchaseRepository,IReportRepository reportRepository)
        {
            this.mapper = mapper;
            this.purchaseRepository = purchaseRepository;
            this.reportRepository = reportRepository;
        }
        public async Task<ReportResponse> Handle(CreateReportByTimeCommand request, CancellationToken cancellationToken)
        {
            
            var report = mapper.Map<Report>(request.Report);
            var purchases =  await purchaseRepository.GetPurchasesByTime(report.ReportTime);

            report.Purchases = purchases.ToList();
            report.Id = Guid.NewGuid();
            return new ReportResponse()
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Successfully created report",
                Report = report,
            };
        }
    }
}
