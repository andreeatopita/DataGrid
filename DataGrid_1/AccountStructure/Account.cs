namespace DataGrid_1.AccountStructure;

public class Account
{
    private readonly List<Transaction> _transactions = new();

    public IReadOnlyCollection<Transaction> Transactions => _transactions;

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
        if(amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        _transactions.Add(new Transaction(amount, Normalize(date), TransactionType.Received));
    }


    //spend - cheltuieste bani din cont
    public void Spend(decimal amount, DateTime? date = null)
    {
        if(amount <= 0 ) 
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        if (amount > Balance)
            throw new InvalidOperationException($"Insufficient funds. Balance={Balance}, spend={amount}");

        _transactions.Add(new Transaction(amount, Normalize(date), TransactionType.Spent));
    }



    //daca data e null sau default, pun data curenta
    private static DateTime Normalize(DateTime? date) =>
                (date is null || date == default) ? DateTime.Now : date.Value;

}