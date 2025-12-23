using BackofficeConsole.DataAccess;
using BackofficeConsole.Services.Contracts;
using BackofficeConsole.Services.Dtos.ReadModelsDto;
using Cocona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackofficeConsole.Services.Dtos.ResultsDto;
using BackofficeConsole.ConsoleUi;
namespace BackofficeConsole.Commands;

internal class StudentCommands
{
    //injectez serviciile in clasa
    private IStudentService Students { get; }
    private IAccountService Accounts { get; }
    // doar ca sa arat istoricul la details

    public StudentCommands(IStudentService students, IAccountService accounts)
    {
        Students = students;
        Accounts = accounts;
    }

    [Command("list", Description = "List students")]
    public async Task ListAsync([Option("only-active", new[] { 'a' })] bool onlyActive = false)
    {
        IReadOnlyList<StudentListDto> rows = await Students.ListAsync(onlyActive);
        PrintTable(rows);
    }

    [Command("details", Description = "Show details & transactions of a student")]
    public async Task DetailsAsync([Option("id", new[] { 'i' })] int id)
    {
        StudentDetailsDto? s = await Students.GetAsync(id);

        if (s is null)
        {
            Console.WriteLine("Student not found.");
            return;
        }

        ConsoleTable.Print(new[] { s }, //ca sa am un singur element de tip StudentDetailsDto, nu 
                ("Id", x => x.StudentId.ToString(), Align.Right),
                ("First Name", x => x.FirstName, Align.Left),
                ("Last Name", x => x.LastName, Align.Left),
                ("Father Name", x => x.FatherName ?? "-", Align.Left),
                ("DOB", x => x.DateOfBirth.ToString("yyyy-MM-dd"), Align.Right),
                ("Age", x => x.AgeYearsMonths, Align.Right),
                ("Active", x => x.IsActive ? "Yes" : "No", Align.Center),
                ("Last Active At", x => x.LastActiveAt.HasValue ? x.LastActiveAt.Value.ToString("yyyy-MM-dd HH:mm") : "-", Align.Right),
                ("Balance", x => x.Balance.ToString("F2"), Align.Right)
            );

        Console.WriteLine();

        IReadOnlyList<StudentTransactionDto> tx = await Accounts.HistoryAsync(id);
        if (tx.Count == 0)
        {
            Console.WriteLine("No transactions.");
            return;
        }

        Console.WriteLine("Transactions:");
        ConsoleTable.Print(
            tx,
            ("Date", t => t.TransactionDate.ToString("yyyy-MM-dd HH:mm"), Align.Center),
            ("Type", t => t.TransactionType, Align.Left),
            ("Amount", t => t.Amount.ToString("F2"), Align.Right)
        );
    }

    [Command("create", Description = "Create a new student")]
    public async Task CreateAsync(
        [Option] string firstName,
        [Option] string lastName,
        [Option] string? fatherName,
        [Option] DateTime dateOfBirth,
        [Option] bool isActive = true)
    {
        Result res = await Students.CreateAsync(firstName, lastName, fatherName, dateOfBirth, isActive);
        Console.WriteLine($"{res.Message} (Id={res.StudentId})");
    }

    [Command("update", Description = "Update an existing student")]
    public async Task UpdateAsync(
        [Option("id", new[] { 'i' })] int studentId,
        [Option] string firstName,
        [Option] string lastName,
        [Option] string? fatherName,
        [Option] DateTime dateOfBirth,
        [Option] bool isActive)
    {
        Result res = await Students.UpdateAsync(studentId, firstName, lastName, fatherName, dateOfBirth, isActive);
        Console.WriteLine($"{res.Message} (Id={res.StudentId})");
    }

    [Command("delete", Description = "Delete a student (fails if has transactions)")]
    public async Task DeleteAsync([Option("id", new[] { 'i' })] int studentId)
    {
        Result res = await Students.DeleteAsync(studentId);
        Console.WriteLine($"{res.Message} (Id={res.StudentId})");
    }

    private static void PrintTable(IEnumerable<StudentListDto> rows)
    {
        ConsoleTable.Print(rows,
            //Id
            ("Id", r => r.StudentId.ToString(), Align.Right),
            //FullName
            ("Full Name", r => r.FullName, Align.Left),
            //DateOfBirth
            ("Date of Birth", r => r.DateOfBirth.ToString("yyyy-MM-dd"), Align.Right),
            //AgeYearsMonths
            ("Age", r => r.AgeYearsMonths, Align.Right),
            //Balance
            ("Balance", r => r.Balance.ToString("F2"), Align.Right),
            //IsActive
            ("Active", r => r.IsActive ? "Yes" : "No", Align.Center),
            //Last Active At
            ("Last Active At", s => s.LastActiveAt.HasValue ? s.LastActiveAt.Value.ToString("yyyy-MM-dd HH:mm"): "-",Align.Right)
         );
    }
}