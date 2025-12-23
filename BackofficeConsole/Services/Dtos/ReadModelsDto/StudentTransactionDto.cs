using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services.Dtos.ReadModelsDto;

//pt istoric
public record StudentTransactionDto(
    DateTime TransactionDate,
    string TransactionType,
    decimal Amount
);
