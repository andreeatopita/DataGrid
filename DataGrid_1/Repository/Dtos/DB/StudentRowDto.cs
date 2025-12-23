using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Repository.Dtos.DB;

internal record StudentRowDto(
    int StudentId,
    string FirstName,
    string LastName,
    string? FatherName,
    DateTime DateOfBirth,
    DateTime? LastActiveAt,
    bool IsActive
);
