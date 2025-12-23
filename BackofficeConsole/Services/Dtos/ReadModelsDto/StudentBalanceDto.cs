using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services.Dtos.ReadModelsDto;

public record StudentsBalanceDto(
    string FirstName,
    string LastName,
    string BirthMonthYear,
    string AgeYearsMonths,
    decimal FormattedBalance,
    string Above400
);
