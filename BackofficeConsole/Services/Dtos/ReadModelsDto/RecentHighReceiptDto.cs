using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.Services.Dtos.ReadModelsDto;

public record RecentHighReceiptDto(
    string NameWithFather,
    DateTime QualifyingTranDate,
    decimal MaxRecAmount
);
