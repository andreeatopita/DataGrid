using BackofficeConsole.Services.Dtos.ReadModelsDto;
using BackofficeConsole.Services.Dtos.ResultsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services.Contracts;

public interface IAccountService
{
    Task<MoneyMovementResult> DepositAsync(int studentId, decimal amount);
    Task<MoneyMovementResult> WithdrawAsync(int studentId, decimal amount);
    Task<IReadOnlyList<StudentTransactionDto>> HistoryAsync(int studentId);

    Task<IReadOnlyList<StudentBalanceIdDto>> ListBalancesAsync();

    Task<decimal?> GetBalanceAsync(int studentId);
}


