using System.Transactions;

namespace AccountLib;

public class Account
{
    //nested class: reprezinta o tranzactie in cont
    private class Transaction
    {
        public decimal Amount { get; }
        public DateTime Date { get; }
        public TransactionType Type { get; }
        public Transaction(decimal amount, DateTime date, TransactionType type)
        {
            // validation aici 
            Amount = amount;
            Date = date;
            Type = type;
        }
    }


    private List<Transaction> Transactions { get; } = new();
    
    public decimal Balance =>
        Transactions.Sum(t => t.Type == TransactionType.Received ? t.Amount : -t.Amount);

    //optional date : daca nu e specificata, se pune data curenta
    //receive - primeste bani in cont
    public void Receive(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException("Amount must be greater than zero.");

        Transactions.Add(new Transaction(amount, DateTime.Now, TransactionType.Received));
    }


    //spend - cheltuieste bani din cont
    public void Spend(decimal amount, DateTime? date = null)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException("Amount must be greater than zero.");

        if (amount > Balance)
            throw new InvalidOperationException($"Insufficient funds. Balance={Balance}, spend={amount}");

        Transactions.Add(new Transaction(amount, DateTime.Now, TransactionType.Spent));
    }



    //daca data e null, pun data curenta
    private static DateTime Normalize(DateTime? date) =>
                (date == null) ? DateTime.Now : date.Value;



    //iau tranzactiile si le transform in TransactionInfo
    //ienumerable: pt citire, ret doar valorile din trans info, citit, nu modificat
    public IEnumerable<TransactionInfo> GetTransactions() => Transactions
        .Select(t => new TransactionInfo (t.Amount, t.Date, t.Type));

    //get received 
    public IEnumerable<TransactionInfo> GetReceived() => Transactions
        .Where(t => t.Type == TransactionType.Received)
        .Select(t => new TransactionInfo (t.Amount, t.Date, t.Type));

    public IEnumerable<TransactionInfo> GetSpent() => Transactions
        .Where(t => t.Type == TransactionType.Spent)
        .Select(t=> new TransactionInfo(t.Amount,t.Date,t.Type));

    //get pentru o anumita data 
    public IEnumerable<TransactionInfo> GetSince(DateTime cutoff) =>
        Transactions.Where(t => t.Date >= cutoff)
                     .Select(t => new TransactionInfo(t.Amount, t.Date, t.Type));

    //get pentru recent received 
    public IEnumerable<TransactionInfo> GetRecentReceivedAbove(decimal minAmount, int days) => Transactions
        .Where(t => t.Type == TransactionType.Received && t.Amount > minAmount && t.Date >= DateTime.Now.AddDays(-days))
                     .Select(t => new TransactionInfo(t.Amount, t.Date, t.Type));


    //pentru incarcare istoric din json 
    //reface tranzactiile exact cu datele din lor 
    public void Replay(IEnumerable<TransactionInfo> history)
    {
        foreach (TransactionInfo? h in history.OrderBy(t => t.Date))
        {
            if (h.Amount <= 0) 
                throw new ArgumentOutOfRangeException("Amount must be > 0");

            if (h.Type == TransactionType.Received)
            {
                Transactions.Add(new Transaction(h.Amount, h.Date, TransactionType.Received));
            }
            else 
            {
                if (h.Amount > Balance)
                    throw new InvalidOperationException("Inconsistent history: spent > balance during replay.");

                Transactions.Add(new Transaction(h.Amount, h.Date, TransactionType.Spent));
            }
        }
    }
}