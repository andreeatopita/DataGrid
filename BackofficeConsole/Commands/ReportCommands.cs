using BackofficeConsole.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackofficeConsole.ConsoleUi;
using Cocona;
using Dapper;
using BackofficeConsole.Services.Dtos.ResultsDto;
using BackofficeConsole.Services.Dtos.ReadModelsDto;

namespace BackofficeConsole.Commands;

//inactive students, high value receipts, students by balance
internal class ReportCommands
{
    //injectez serviciul
    private IReportService Report { get; }

    public ReportCommands(IReportService report)
    {
        Report = report;
    }

    [Command("inactive-students", Description = "List inactive students")]
    public async Task InactiveStudentsAsync()
    {
        IReadOnlyList<InactiveStudentDto> rows = await Report.InactiveStudentsAsync();
        if (rows.Count == 0)
        {
            Console.WriteLine("No inactive students found.");
            return;
        }

        ConsoleTable.Print(
             rows,
            ("Name", x => x.NameWithFather, Align.Left),
            ("Birth Month/Year", x => x.BirthMonthYear, Align.Center),
            ("Age", x => x.AgeYearsMonths, Align.Right),
            ("Last Active At", x => x.LastActiveAt?.ToString("yyyy-MM-dd") ?? "N/A", Align.Center)
        );
    }

    [Command("students-balance", Description = "List students by account balance")]
    public async Task StudentsByBalanceAsync()
    {
        IReadOnlyList<StudentsBalanceDto> rows = await Report.StudentsByBalanceAsync();
        if (rows.Count == 0)
        {
            Console.WriteLine("No balance data found.");
            return;
        }

        ConsoleTable.Print(
             rows,
            ("First Name", x => x.FirstName, Align.Left),
            ("Last Name", x => x.LastName, Align.Left),
            ("Birth Month/Year", x => x.BirthMonthYear, Align.Center),
            ("Age", x => x.AgeYearsMonths, Align.Right),
            ("Account Balance", x => x.FormattedBalance.ToString("C"), Align.Right),
            ("Above 400", x => x.Above400, Align.Center)
        );
    }

    [Command("high-value-receipts", Description = "List high value receipts")]
    public async Task HighValueReceiptsAsync()
    {
        IReadOnlyList<RecentHighReceiptDto> rows = await Report.RecentHighReceiptsAsync();
        if (rows.Count == 0)
        {
            Console.WriteLine("No high value receipts found.");
            return;
        }
        ConsoleTable.Print(
             rows,
            ("Name", x => x.NameWithFather, Align.Left),
            ("Transaction Date", x => x.QualifyingTranDate.ToString("yyyy-MM-dd"), Align.Center),
            ("Max Receipt Amount", x => x.MaxRecAmount.ToString("C"), Align.Right)
        );
    }
}
