using BackofficeConsole.Services.Dtos.ReadModelsDto;
using BackofficeConsole.Services.Dtos.ResultsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Domain.Repositories;

public interface IAccountRepository
{
    Task<MoneyMovementResult> DepositAsync(int studentId, decimal amount);
    Task<MoneyMovementResult> WithdrawAsync(int studentId, decimal amount);
    Task<IReadOnlyList<StudentBalanceIdDto>> ListBalancesAsync();
}


