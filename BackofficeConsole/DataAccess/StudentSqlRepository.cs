using BackofficeConsole.Domain.Repositories;
using BackofficeConsole.Services.Dtos.ReadModelsDto;
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

namespace BackofficeConsole.DataAccess;
//dataaccces pt scriere
//singleton in program
public class StudentSqlRepository : IStudentReadRepository, IStudentWriteRepository
{
    private readonly string cs;
    public StudentSqlRepository(string connectionString) => cs = connectionString;


    //primisiune ca returneaza <> 
    public async Task<Result> CreateStudentAsync(string firstName, string lastName, string? fatherName, DateTime dateOfBirth, bool isActive)
    {
        await using SqlConnection conn = new SqlConnection(cs);
        await conn.OpenAsync();
        await using DbTransaction transaction = await conn.BeginTransactionAsync();

        try
        {
            //ob anonim cu parametrii pt sp,in contextul tranzactiei actuale, tip comanda sp
            //querysingle pt ca returneaza un singur rand, std create returneaza id si mesaj
            Result res = await conn.QuerySingleAsync<Result>("dbo.Student_Create",
                new { FirstName = firstName, LastName = lastName, FatherName = fatherName, DateOfBirth = dateOfBirth, IsActive = isActive },
                transaction: transaction, commandType: CommandType.StoredProcedure);

            await transaction.CommitAsync();

            //dapper ia coloaenele din rand si le mapeaza in proprietatile din dto
            //returnez rezultatul
            return res;
        }
        catch (SqlException ex)
        {
            //rollback : nu ramana neterminat in db
            await transaction.RollbackAsync();

            //returnez eroarea, mesajul si id-ul 0
            //mesajele aruncate cu throw din sp 
            return new Result(ex.Message, 0);
        }
    }
    public async Task<Result> UpdateStudentAsync(int studentId, string firstName, string lastName, string? fatherName, DateTime dateOfBirth, bool isActive)
    {
        await using SqlConnection conn = new SqlConnection(cs);
        await conn.OpenAsync();
        await using DbTransaction transaction = await conn.BeginTransactionAsync();

        try
        {

            Result res = await conn.QuerySingleAsync<Result>("dbo.Student_Update",
                new { StudentId = studentId, FirstName = firstName, LastName = lastName, FatherName = fatherName, DateOfBirth = dateOfBirth, IsActive = isActive },
                transaction: transaction, commandType: CommandType.StoredProcedure);

            //dapper ia coloaenele din rand si le mapeaza in proprietatile din dto
            await transaction.CommitAsync();
            return res;
        }
        catch (SqlException ex)
        {
            await transaction.RollbackAsync();
            return new Result(ex.Message, studentId);
        }
    }

    public async Task<Result> DeleteStudentAsync(int studentId)
    {
        await using SqlConnection conn = new SqlConnection(cs);
        await conn.OpenAsync();
        await using DbTransaction transaction = await conn.BeginTransactionAsync();

        //ob anonim cu param pt procedura 
        try
        {
            Result res = await conn.QuerySingleAsync<Result>("dbo.Student_Delete",
                new { StudentId = studentId },
                transaction: transaction, commandType: CommandType.StoredProcedure);

            await transaction.CommitAsync();
            return res;
        }
        catch (SqlException ex)
        {
            await transaction.RollbackAsync();
            return new Result(ex.Message, studentId);
        }
    }

    //map rez in dto si intoarce lista/obiecte catre servicii
    public async Task<IReadOnlyList<StudentListDto>> ListStudentsAsync()
    {
        const string sql = @"
            SELECT i.StudentId,
                   i.FullName,
                   i.DateOfBirth,
                   i.AgeYearsMonths,
                   b.Balance,
                   i.IsActive,
                   i.LastActiveAt
            FROM View_StudentInfo AS i
            LEFT JOIN View_StudentBalance AS b ON b.StudentId = i.StudentId";

        await using SqlConnection conn = new SqlConnection(cs);
        //trimite sql la db si map la dto
        IEnumerable<StudentListDto> rows = await conn.QueryAsync<StudentListDto>(sql);
        //primeste randurile inapoi, pt fiecare rand map la dto
        //queryasnc returneaza un IEnumerable, tolist pt a converti la lista
        return rows.ToList();
    }

    public async Task<StudentDetailsDto?> GetStudentByIdAsync(int id)
    {
        const string sql = @"
        SELECT s.StudentId,
               s.FirstName,
               s.LastName,
               s.FatherName,
               s.DateOfBirth,
               i.AgeYearsMonths,
               s.IsActive,
               s.LastActiveAt,
               b.Balance AS Balance
        FROM Students s
        LEFT JOIN View_StudentInfo i ON i.StudentId = s.StudentId
        LEFT JOIN View_StudentBalance b ON b.StudentId = s.StudentId
        WHERE s.StudentId = @id;";

        //zero sau un rand 
        await using SqlConnection conn = new SqlConnection(cs);
        //ob anonim cu proprietatea id, pt a trimite parametrul in query
        return await conn.QuerySingleOrDefaultAsync<StudentDetailsDto>(sql, new { id });
        //daca nu gaseste niciun rand, returneaza null
    }

    //istoric tranzactii pt un student 
    public async Task<IReadOnlyList<StudentTransactionDto>> ListTransactionsForStudentAsync(int studentId)
    {
        const string sql = @"
            SELECT t.TransactionDate,
                   t.TransactionType,
                   t.Amount
            FROM Students s
            JOIN Accounts a ON a.StudentId = s.StudentId
            JOIN StudentTransactions t ON t.AccountId = a.AccountId
            WHERE s.StudentId = @studentId
            ORDER BY t.TransactionDate, t.TransactionId;";

        await using var conn = new SqlConnection(cs);
        IEnumerable<StudentTransactionDto> rows = await conn.QueryAsync<StudentTransactionDto>(sql, new { studentId });
        return rows.ToList();
    }

}
