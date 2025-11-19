using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DataGrid_1.Repository.Dto;

namespace DataGrid_1.Repository;


public class JsonStudentRepo : IJsonRepo<Student>
{
    private readonly string filePath;
    
    private readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    //doar o operatie poate citi/scrie in fisier la un moment dat
    //1- cate sunt libere la inceput, 1- cate pot fi max
    private static readonly SemaphoreSlim semaphore = new(1, 1);

    public JsonStudentRepo(string path)
    {
        filePath = path;
    }


    public async Task<IReadOnlyList<Student>> LoadAsync()
    {
        throw new NotImplementedException();
    }

    public async Task SaveAsync(IEnumerable<Student> items)
    {
        throw new NotImplementedException();
    }


    //serializare pt json
    //iau student( account privat ) si in transform in dto ( cu lista de tranzactii publice )
    private static StudentDto MapToDto(Student s) =>
        new StudentDto(
            s.StudentId,
            s.FirstName,
            s.LastName,
            s.FatherName,
            s.DateOfBirth,
            s.LastActiveAt,
            s.IsActive,
            s.AllTransactions()
                .Select(tr => new TransactionDto(tr.Amount, tr.Date, tr.Type.ToString()))
                .ToList()
        );
    //tolist : pt ca AllTransactions returneaza IEnumerable


    //map to entity
    //deserializare din json
    private static Student MapToEntity(StudentDto dto)
    {
        Student s=new Student(dto.StudentId, dto.FirstName, dto.LastName, dto.DateOfBirth,dto.IsActive, dto.FatherName,dto.LastActiveAt, 0);

        //daca nu are tranzactii, initializez cu lista vida
        List<TransactionDto> trs = dto.Transactions ?? new List<TransactionDto>();   

        foreach (TransactionDto tran in trs.OrderBy(t=>t.Date)) //pt a fi crnologic
        {
            //reproduc tranzactia in cont
            if (tran.Type.Equals("Received",StringComparison.OrdinalIgnoreCase))
            {
                s.ReceiveMoney(tran.Amount, tran.Date);
            }
            else if (tran.Type.Equals("Spent",StringComparison.OrdinalIgnoreCase))
            {
                s.SpendMoney(tran.Amount, tran.Date);
            }
        }
        return s;
    }
}

