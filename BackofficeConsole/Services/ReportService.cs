using BackofficeConsole.DataAccess;
using BackofficeConsole.Domain.Repositories;
using BackofficeConsole.Services.Contracts;
using BackofficeConsole.Services.Dtos.ReadModelsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services;

//ce vrea aplicatia sa faca, iar repo doar sa interactioneze cu db
public class ReportService : IReportService
{
    private IReportRepository Report { get; }
    public ReportService(IReportRepository report) => Report = report;

    public Task<IReadOnlyList<InactiveStudentDto>> InactiveStudentsAsync() => Report.ListInactiveStudentsAsync();

    public Task<IReadOnlyList<StudentsBalanceDto>> StudentsByBalanceAsync() => Report.ListStudentsByBalanceAsync();

    public Task<IReadOnlyList<RecentHighReceiptDto>> RecentHighReceiptsAsync() => Report.ListRecentHighValueReceiptsAsync();
}