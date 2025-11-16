using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.AccountStructure;

//tip tranzactie: depunere/retragere
public class Transaction
{
    public decimal Amount { get; }
    public DateTime Date { get; }
    public TransactionType Type { get; }

    public Transaction(decimal amount, DateTime date, TransactionType type)
    {
        if(amount<= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        Date = date;

        Amount = amount;
        Type = type;
    }
}
