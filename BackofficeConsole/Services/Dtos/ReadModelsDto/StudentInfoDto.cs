using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services.Dtos.ReadModelsDto;

public record StudentInfoDto(
    int StudentId,
    string FullName,
    DateTime DateOfBirth,
    string BirthMonthYear,
    string AgeYearsMonths,
    bool IsActive,
    DateTime? LastActiveAt
);
