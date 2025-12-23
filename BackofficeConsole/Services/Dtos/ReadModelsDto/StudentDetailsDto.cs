using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services.Dtos.ReadModelsDto;

public record StudentDetailsDto(
    int StudentId,
    string FirstName,
    string LastName,
    string? FatherName,
    DateTime DateOfBirth,
    string AgeYearsMonths,
    bool IsActive,
    DateTime? LastActiveAt,
    decimal Balance
);