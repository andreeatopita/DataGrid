using BackofficeConsole.Services.Dtos.ReadModelsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Domain.Repositories;

public interface IReportRepository
{
    Task<IReadOnlyList<InactiveStudentDto>> ListInactiveStudentsAsync();
    Task<IReadOnlyList<StudentsBalanceDto>> ListStudentsByBalanceAsync();
    Task<IReadOnlyList<RecentHighReceiptDto>> ListRecentHighValueReceiptsAsync();
}
