using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services.Dtos.ReadModelsDto;

public record StudentListDto(
    int StudentId,
    string FullName,
    DateTime DateOfBirth,
    string AgeYearsMonths,
    decimal Balance,
    bool IsActive,
    DateTime? LastActiveAt
);
