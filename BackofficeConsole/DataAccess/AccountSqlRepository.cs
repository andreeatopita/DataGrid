using BackofficeConsole.Domain.Repositories;
using BackofficeConsole.Services.Dtos.ResultsDto;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using BackofficeConsole.Services.Dtos.ReadModelsDto;

namespace BackofficeConsole.DataAccess;

internal class AccountSqlRepository : IAccountRepository
{
    private readonly string cs;
    public AccountSqlRepository(string connectionString) => cs = connectionString;

    //daca e ok : succes true si mesaje + balance, daca exista eroare : false si mesajul
    public async Task<MoneyMovementResult> DepositAsync(int studentId, decimal amount)
    {
        await using SqlConnection conn = new SqlConnection(cs);
        await conn.OpenAsync();
        await using DbTransaction transaction = await conn.BeginTransactionAsync();

        try
        {
            MoneyMovementResult res = await conn.QuerySingleAsync<MoneyMovementResult>("dbo.Account_Deposit",
                new { StudentId = studentId, Amount = amount },
                transaction: transaction, commandType: CommandType.StoredProcedure);

            await transaction.CommitAsync();
            return res;
        }
        catch (SqlException ex)
        {
            await transaction.RollbackAsync();
            return new MoneyMovementResult(false, ex.Message, studentId, 0m, 0m);
        }
    }

    public async Task<IReadOnlyList<StudentBalanceIdDto>> ListBalancesAsync()
    {
        const string sql = @"
            SELECT StudentId, Balance
            FROM View_StudentBalance;";

        await using SqlConnection conn = new SqlConnection(cs);
        IEnumerable<StudentBalanceIdDto> rows = await conn.QueryAsync<StudentBalanceIdDto>(sql);
        return rows.ToList();
    }

    //si cu id
    public async Task<IReadOnlyList<StudentBalanceIdDto>> ListBalancesAsync(int studentId)
    {
        const string sql = @"
            SELECT StudentId, Balance
            FROM View_StudentBalance
            WHERE StudentId = @StudentId;";

        await using SqlConnection conn = new SqlConnection(cs);
        IEnumerable<StudentBalanceIdDto> rows = await conn.QueryAsync<StudentBalanceIdDto>(sql, new { StudentId = studentId });
        return rows.ToList();
    }


    public async Task<MoneyMovementResult> WithdrawAsync(int studentId, decimal amount)
    {
        await using SqlConnection conn = new SqlConnection(cs);
        await conn.OpenAsync();
        await using DbTransaction transaction = await conn.BeginTransactionAsync();

        try
        {
            MoneyMovementResult res = await conn.QuerySingleAsync<MoneyMovementResult>("dbo.Account_Withdraw",
                new { StudentId = studentId, Amount = amount },
                transaction: transaction, commandType: CommandType.StoredProcedure);

            await transaction.CommitAsync();
            return res;
        }
        catch (SqlException ex)
        {
            await transaction.RollbackAsync();
            return new MoneyMovementResult(false, ex.Message, studentId, 0m, 0m);
        }
    }
}
