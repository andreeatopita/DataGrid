using BackofficeConsole.Services.Dtos.ResultsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Domain.Repositories;

public interface IStudentWriteRepository
{
    Task<Result> CreateStudentAsync(string firstName, string lastName, string? fatherName, DateTime dateOfBirth, bool isActive);
    Task<Result> UpdateStudentAsync(int studentId, string firstName, string lastName, string? fatherName, DateTime dateOfBirth, bool isActive);
    Task<Result> DeleteStudentAsync(int studentId);

}
