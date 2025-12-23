using BackofficeConsole.DataAccess;
using BackofficeConsole.Domain.Repositories;
using BackofficeConsole.Services.Contracts;
using BackofficeConsole.Services.Dtos.ReadModelsDto;
using BackofficeConsole.Services.Dtos.ResultsDto;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services;

//cereri de la commands, fol repo ca sa ia , scrie date , aplica reguli
public class AccountService : IAccountService
{
    private IAccountRepository Account { get; }
    private IStudentReadRepository StudentRead { get; }
    public AccountService(IAccountRepository account, IStudentReadRepository studentRead)
    {
        Account = account;
        StudentRead = studentRead;
    }

    public Task<MoneyMovementResult> DepositAsync(int studentId, decimal amount) => Account.DepositAsync(studentId, amount);

    public Task<MoneyMovementResult> WithdrawAsync(int studentId, decimal amount) => Account.WithdrawAsync(studentId, amount);

    public Task<IReadOnlyList<StudentTransactionDto>> HistoryAsync(int studentId) => StudentRead.ListTransactionsForStudentAsync(studentId);

    public Task<IReadOnlyList<StudentBalanceIdDto>> ListBalancesAsync() => Account.ListBalancesAsync();

    public async Task<decimal?> GetBalanceAsync(int studentId)
    {
        StudentDetailsDto? s = await StudentRead.GetStudentByIdAsync(studentId);
        return s?.Balance;
    }
}