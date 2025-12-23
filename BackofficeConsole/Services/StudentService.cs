using BackofficeConsole.DataAccess;
using BackofficeConsole.Domain.Repositories;
using BackofficeConsole.Services.Contracts;
using BackofficeConsole.Services.Dtos.ReadModelsDto;
using BackofficeConsole.Services.Dtos.ResultsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services;

public class StudentService : IStudentService
{
    private IStudentReadRepository StudentRead { get; }
    private IStudentWriteRepository StudentWrite { get; }

    public StudentService(IStudentReadRepository studentRead, IStudentWriteRepository studentWrite)
    {
        StudentRead = studentRead;
        StudentWrite = studentWrite;
    }   

    public async Task<IReadOnlyList<StudentListDto>> ListAsync(bool onlyActive = false)
    {
        var rows = await StudentRead.ListStudentsAsync();
        if(onlyActive == false)
        {
            return rows;
        }
        else
        {
            return rows.Where(x => x.IsActive).ToList();
        }
    }

    public Task<StudentDetailsDto?> GetAsync(int studentId) => StudentRead.GetStudentByIdAsync(studentId);

    public Task<Result> CreateAsync(string firstName, string lastName, string? fatherName, DateTime dateOfBirth, bool isActive)
        => StudentWrite.CreateStudentAsync(firstName, lastName, fatherName, dateOfBirth, isActive);

    public Task<Result> UpdateAsync(int studentId, string firstName, string lastName, string? fatherName,DateTime dateOfBirth, bool isActive)
        => StudentWrite.UpdateStudentAsync(studentId, firstName, lastName, fatherName, dateOfBirth, isActive);

    public Task<Result> DeleteAsync(int studentId) => StudentWrite.DeleteStudentAsync(studentId);
}