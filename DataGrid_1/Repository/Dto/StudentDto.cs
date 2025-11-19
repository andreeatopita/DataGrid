using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Repository.Dto;

public record TransactionDto(
    decimal Amount,
    DateTime Date,
    string Type     //received/Spent
);
//enum

public record StudentDto(
int StudentId,
string FirstName,
string LastName,
string? FatherName,
DateTime DateOfBirth,
DateTime LastActiveAt,
bool IsActive,
List<TransactionDto> Transactions
);