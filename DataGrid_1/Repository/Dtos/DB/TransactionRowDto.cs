using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Repository.Dtos.DB;

internal record TransactionRowDto(
    int TransactionId,
    int AccountId,
    decimal Amount,
    DateTime TransactionDate,
    string TransactionType
);
