using Dapper;
using DataGrid_1.Repository.Dtos.DB;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using DataGridLib.Contracts;

namespace DataGrid_1.Repository;
//aici:citirea din baza de date 
public class SqlRepository : IRepository<Student>
{
    private readonly string connectionString;
    public SqlRepository(string _connectionString) => connectionString = _connectionString;

    public async Task<IReadOnlyList<Student>> LoadAsync()
    {
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        //load students
        const string studentsSql = @"
            SELECT StudentId, FirstName, LastName, FatherName, DateOfBirth, LastActiveAt, IsActive
            FROM dbo.Students";

        List<StudentRowDto> rows = (await conn.QueryAsync<StudentRowDto>(studentsSql)).AsList();

        //load balances
        const string balanceSql = @"SELECT StudentId, Balance FROM dbo.View_StudentBalance;";

        //aici incarc balantele in dictionar
        Dictionary<int, decimal> balances = (await conn.QueryAsync<(int StudentId, decimal Balance)>(balanceSql))
               .ToDictionary(x => x.StudentId, x => x.Balance);

        //map to domain entities
        var result = new List<Student>(rows.Count);

        foreach(var r in rows)
        {
            //iau balanta din dictionar pt fiecare student, daca nu, 0
            balances.TryGetValue(r.StudentId, out decimal balance);

            //entitatea mea Student
            var s =new Student(
                id: r.StudentId,
                firstName: r.FirstName,
                lastName: r.LastName,
                dateOfBirth: r.DateOfBirth,
                isActive: r.IsActive,
                fatherName: r.FatherName,
                lastActiveAt: r.LastActiveAt ?? default,
                initialBalance: balance 
                );
            result.Add(s);
        }
        return result;
    }
}
