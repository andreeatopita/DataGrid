using DataGrid_1.Repository.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Repository;

public interface IStudentReportRepository
{
    Task<IReadOnlyList<InactiveStudentDto>> LoadInactiveAsync();
    Task<IReadOnlyList<StudentBalanceDto>> LoadBalanceAboveAsync();
    Task<IReadOnlyList<RecentHighReceiptDto>> LoadRecentHighReceiptsAsync();
}
