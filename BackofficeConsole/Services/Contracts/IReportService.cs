using BackofficeConsole.Services.Dtos.ReadModelsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services.Contracts;

public interface IReportService
{
    Task<IReadOnlyList<InactiveStudentDto>> InactiveStudentsAsync();
    Task<IReadOnlyList<StudentsBalanceDto>> StudentsByBalanceAsync();
    Task<IReadOnlyList<RecentHighReceiptDto>> RecentHighReceiptsAsync();
}