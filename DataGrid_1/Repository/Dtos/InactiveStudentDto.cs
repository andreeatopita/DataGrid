using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Repository.Dtos;

public record InactiveStudentDto(
    string FullName,
    DateTime DateOfBirth,
    string AgeYearsMonths,
    DateTime? LastActiveAt
);