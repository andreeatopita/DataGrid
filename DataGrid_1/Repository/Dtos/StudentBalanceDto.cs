using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Repository.Dtos;

public record StudentBalanceDto(
    string FirstName,
    string LastName,
    string BirthMonthYear,
    string AgeYearsMonths,
    string FormattedBalance,
    string Above400
);