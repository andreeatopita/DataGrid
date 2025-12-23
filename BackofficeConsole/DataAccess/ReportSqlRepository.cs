using BackofficeConsole.Domain.Repositories;
using BackofficeConsole.Services.Dtos.ReadModelsDto;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace BackofficeConsole.DataAccess;

internal class ReportSqlRepository : IReportRepository
{
    private readonly string cs;
    public ReportSqlRepository(string connectionString) => cs = connectionString;

    public async Task<IReadOnlyList<InactiveStudentDto>> ListInactiveStudentsAsync()
    {
        const string sql = @"
            SELECT * FROM Report_InactiveStudents
            ORDER BY LastActiveAt DESC";

        await using SqlConnection conn = new SqlConnection(cs);
        IEnumerable<InactiveStudentDto> rows = await conn.QueryAsync<InactiveStudentDto>(sql);
        return rows.ToList();
    }

    public async Task<IReadOnlyList<StudentsBalanceDto>> ListStudentsByBalanceAsync()
    {
        const string sql = @"
            SELECT
            FirstName,
            LastName,           
            BirthMonthYear,
            AgeYearsMonths,
            FormattedBalance,
            Above400
        FROM Report_StudentsByBalance
        ORDER BY DateOfBirth DESC;";
        //date of birth in select ca sa poata fi mapat in dto

        await using SqlConnection conn = new SqlConnection(cs);
        IEnumerable<StudentsBalanceDto> rows = await conn.QueryAsync<StudentsBalanceDto>(sql);
        return rows.ToList();
    }


    public async Task<IReadOnlyList<RecentHighReceiptDto>> ListRecentHighValueReceiptsAsync()
    {
        const string sql = @"
            SELECT
                NameWithFather,
                QualifyingTranDate,
                MaxRecAmount
            FROM Report_RecentHighValueReceipts
            ORDER BY MaxRecAmount DESC";

        await using SqlConnection conn = new SqlConnection(cs);
        IEnumerable<RecentHighReceiptDto> rows = await conn.QueryAsync<RecentHighReceiptDto>(sql);
        return rows.ToList();
    }
}
