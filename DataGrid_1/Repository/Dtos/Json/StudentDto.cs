using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Repository.Dtos.Json;

//ca sa trimiti/primesti doar datele de care ai nevoie in forma stabila si sigura, fara sa expun direct entitatea interna ( modelul EF )
internal record StudentDto(
    int StudentId,
    string FirstName,
    string LastName,
    string? FatherName,
    DateTime DateOfBirth,
    DateTime? LastActiveAt,
    bool IsActive,
    List<TransactionDto> Transactions
);