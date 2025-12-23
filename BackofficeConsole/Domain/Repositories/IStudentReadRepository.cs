using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackofficeConsole.Services.Dtos.ReadModelsDto;


namespace BackofficeConsole.Domain.Repositories;

public interface IStudentReadRepository
{
    Task<IReadOnlyList<StudentListDto>> ListStudentsAsync();
    Task<StudentDetailsDto?> GetStudentByIdAsync(int id);
    Task<IReadOnlyList<StudentTransactionDto>> ListTransactionsForStudentAsync(int studentId);
}
