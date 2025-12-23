using BackofficeConsole.ConsoleUi;
using BackofficeConsole.Services.Contracts;
using BackofficeConsole.Services.Dtos.ReadModelsDto;
using BackofficeConsole.Services.Dtos.ResultsDto;
using Cocona;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Commands;

//deposit, withdraw, balances
internal class AccountCommands
{
    //interfete, nu implementari concrete 
    //ce are nevoie, nu cum creez
    //ce am nevoie , nu cum il creez
    private IAccountService Accounts { get; }
    private IStudentService Students { get; }

    public AccountCommands(IAccountService accounts, IStudentService students)
    {
        Accounts = accounts;
        Students = students;
    }

    [Command("deposit", Description = "Deposit money into a student's account")]
    public async Task DepositAsync([Option("id", new[] { 'i' })] int studentId, [Option("amount", new[] { 'a' })] decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Amount must be positive.");
            return;
        }

        MoneyMovementResult res = await Accounts.DepositAsync(studentId, amount);

        Console.WriteLine($"{res.Message} (Id={res.StudentId}) ");

        if(res.IsSuccess)
            Console.WriteLine($"Balance: {res.CurrentBalance:F2} -> {res.NewBalance:F2}");
    }

    [Command("withdraw", Description = "Withdraw money from a student's account")]
    public async Task WithdrawAsync([Option("id", new[] { 'i' })] int studentId, [Option("amount", new[] { 'a' })] decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Amount must be positive.");
            return;
        }

        MoneyMovementResult res = await Accounts.WithdrawAsync(studentId, amount);

        Console.WriteLine($"{res.Message} (Id={res.StudentId}) ");
        if(res.IsSuccess)
            Console.WriteLine($"Balance: {res.CurrentBalance:F2} -> {res.NewBalance:F2}");

    }

    [Command("balance", Description = "Show balance for a single student")]
    public async Task BalanceAsync([Option("id", new[] { 'i' })] int studentId)
    {
        decimal? balance = await Accounts.GetBalanceAsync(studentId);

        if (balance is null)
        {
            Console.WriteLine($"Student {studentId} not found or has no account.");
            return;
        }

        Console.WriteLine($"Student {studentId} balance: {balance.Value:F2}");

    }

    [Command("balances", Description = "Show balances for all students")]
    public async Task BalancesAsync()
    {
        IReadOnlyList<StudentBalanceIdDto> rows = await Accounts.ListBalancesAsync();

        if (rows.Count == 0)
        {
            Console.WriteLine("No students found.");
            return;
        }

        ConsoleTable.Print(
            rows,
            ("Id", x => x.StudentId.ToString(), Align.Right),
            ("Balance", x => x.Balance.ToString("F2"), Align.Right)
            );
    }

    [Command("history", Description = "Show transaction history for a student")]
    public async Task HistoryAsync([Option("id", new[] { 'i' })] int studentId)
    {
        IReadOnlyList<StudentTransactionDto> transactionHistory = await Accounts.HistoryAsync(studentId);

        if (transactionHistory.Count == 0)
        {
            Console.WriteLine("No transactions found for the student.");
            return;
        }

        ConsoleTable.Print(
            transactionHistory,
            ("Date", t => t.TransactionDate.ToString("yyyy-MM-dd HH:mm"), Align.Center),
            ("Type", t => t.TransactionType, Align.Left),
            ("Amount", t => t.Amount.ToString("F2"), Align.Right)
        );

    }
}
