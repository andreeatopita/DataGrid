using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Repository.Dtos;

public record RecentHighReceiptDto(
    string NameWithFather,
    DateTime QualifyingTranDate,
    decimal MaxRecAmount
);