using DataGrid_1.AccountStructure;
using DataGrid_1.Formatting;
using DataGrid_1.Formatting.Interfaces;
using DataGridLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Grids;

//>100 in ult 30 de zile

public class RecentHighValueTransactionsGrid : IGridBuilder<Student>
{
    public decimal MinAmount { get; }
    public int Days { get; }

    public CultureInfo Culture { get; }

    private ICurrencyFormatter Currency { get; }

    public RecentHighValueTransactionsGrid(decimal amount = 100, int days = 30, CultureInfo? culture = null)
    {
        MinAmount = amount;
        Days = days;
        //daca cultura e null, folosesc cultura curenta, daca nu e null, folosesc cultura primita
        Culture = culture ?? CultureInfo.CurrentCulture;
        Currency = CurrencyFormatter.CreateFromCulture(Culture);
        //creeaza pe baza culturii
    }
    public GridConfiguration<Student> Build()
    {
        return new GridConfiguration<Student>()
            .RowNumber(false)
            .Where(HasQualifying) //filtrez studentii care au tranzactii care indeplinesc conditiile
            .OrderBy(MaxQualifyingAmount, desc: true)
            //ordonez desc dupa suma maxima a tranzactiilor care indeplinesc conditiile
            .AddColumn("Full Name", s => s.NameWithFather())
            .AddColumn("Transaction Date", s => MaxQualifyingDate(s),
            cellFormatter: dt => dt.ToString("yyyy-MM-dd"))
            //returneaza data tranzactiei cu suma maxima care indeplineste conditiile
            .AddColumn("Max Received", s => MaxQualifyingAmount(s),
            cellFormatter: amount => Currency.FormatCurrency(amount)); //currency : formateaza suma in functie de cultura
    }

    //verific daca studentul are tranzactii care indeplinesc conditiile
    private bool HasQualifying(Student s) =>
        s.Account.Transactions.Any(t =>
            t.Type == TransactionType.Received && //doar tranzactii de tip received
            t.Amount > MinAmount && //suma mai mare decat MinAmount
            t.Date >= DateTime.Now.AddDays(-Days)); //in ultimele Days zile

    //returneaza suma maxima a tranzactiilor care indeplinesc conditiile
    private decimal MaxQualifyingAmount(Student s) =>
        s.Account.Transactions
            .Where(t => t.Type == TransactionType.Received &&
                        t.Amount > MinAmount &&
                        t.Date >= DateTime.Now.AddDays(-Days))
            .Select(t => t.Amount)
            .DefaultIfEmpty(0m) //daca nu exista tranzactii, returneaza 0
            .Max(); //returneaza val maxima

    //returneaza data tranzactiei cu suma maxima care indeplineste conditiile
    private DateTime MaxQualifyingDate(Student s)
    {
        var tx = s.Account.Transactions
            .Where(t => t.Type == TransactionType.Received &&
                        t.Amount > MinAmount &&
                        t.Date >= DateTime.Now.AddDays(-Days))
            .OrderByDescending(t => t.Amount) //ordonez desc dupa suma
            .ThenByDescending(t => t.Date) //desc dupa data
            .FirstOrDefault(); //iau prima tranz sau null daca nu exista niciun element
        return tx?.Date ?? default;
        //daca tx nu e null iau tx.date , daca e null, rez e null => ?? default 
    }
}
