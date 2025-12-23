using BackofficeConsole.Services.Dtos.ReadModelsDto;
using BackofficeConsole.Services.Dtos.ResultsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services.Contracts;

public interface IStudentService
{
    Task<IReadOnlyList<StudentListDto>> ListAsync(bool onlyActive = false);
    Task<StudentDetailsDto?> GetAsync(int studentId);

    Task<Result> CreateAsync(string firstName, string lastName, string? fatherName, DateTime dateOfBirth, bool isActive);
    Task<Result> UpdateAsync(int studentId, string firstName, string lastName, string? fatherName, DateTime dateOfBirth, bool isActive);
    Task<Result> DeleteAsync(int studentId);
}