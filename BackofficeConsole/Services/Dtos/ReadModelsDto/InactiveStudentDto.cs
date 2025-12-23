using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services.Dtos.ReadModelsDto;

public record InactiveStudentDto(
    string NameWithFather,
    string BirthMonthYear,
    string AgeYearsMonths,
    DateTime? LastActiveAt
);

//dapper mapeaza by name 
