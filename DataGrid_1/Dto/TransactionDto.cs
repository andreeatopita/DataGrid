using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Dto;

public class TransactionDto
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    //received/spent
    public string Type { get; set; } = string.Empty; 
}
