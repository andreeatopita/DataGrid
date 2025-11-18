using System.Transactions;

namespace AccountLib;

public class Account
{
    //nested class
    private class Transaction
    {
        public decimal Amount { get; }
        public DateTime Date { get; }
        public TransactionType Type { get; }
        public Transaction(decimal amount, DateTime date, TransactionType type)
        {
            Amount = amount;
            Date = date;
            Type = type;
        }
    }


    private readonly List<Transaction> _transactions = new();
    
    //sold = total intrari - total iesiri ( calculat din istoric)
    //dinamic
    //public decimal Balance =>
    //    _transactions.Where(t => t.Type == TransactionType.Received).Sum(t => t.Amount) - _transactions.Where(t => t.Type == TransactionType.Spent).Sum(t => t.Amount);

    //total incasari - total cheltuieli
    public decimal Balance =>
        _transactions.Sum(t => t.Type == TransactionType.Received ? t.Amount : -t.Amount);

    //optional date : daca nu e specificata, se pune data curenta
    //receive - primeste bani in cont
    public void Receive(decimal amount, DateTime? date = null)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");

        _transactions.Add(new Transaction(amount, Normalize(date), TransactionType.Received));
    }


    //spend - cheltuieste bani din cont
    public void Spend(decimal amount, DateTime? date = null)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");

        if (amount > Balance)
            throw new InvalidOperationException($"Insufficient funds. Balance={Balance}, spend={amount}");

        _transactions.Add(new Transaction(amount, Normalize(date), TransactionType.Spent));
    }



    //daca data e null, pun data curenta
    private static DateTime Normalize(DateTime? date) =>
                (date == null) ? DateTime.Now : date.Value;


    //safe read only get transactions
}