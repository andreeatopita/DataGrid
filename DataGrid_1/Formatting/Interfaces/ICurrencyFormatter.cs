using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Formatting.Interfaces;

public interface ICurrencyFormatter
{
    string FormatCurrency(decimal amount);
}
