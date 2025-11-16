using DataGridLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataGrid_1.Formatting.Interfaces;
using DataGrid_1.Formatting;

namespace DataGrid_1.Grids;

public class StudentsByBalanceGrid : IGridBuilder<Student>
{
    //Dependency inversion : depinde de ICurrencyFormatter, nu depinde de implementarea specifica
    //Open/Closed: pot schimba implementarea fara sa modific logica clasei
    private ICurrencyFormatter Currency { get; }
    //cultura aleasa pentru afisare...
    private CultureInfo Culture { get; }

    public StudentsByBalanceGrid(CultureInfo? culture=null, string? prefix = null, string? suffix=null)
    {
        //daca cultura e null, folosesc cultura curenta, daca nu e null, folosesc cultura primita
        Culture = culture ?? CultureInfo.CurrentCulture;
        
        //daca prefix si suffix sunt null, folosesc formatarea default 
        //daca nu sunt null, folosesc prefixul si suffixul primite
        if(prefix == null && suffix == null)
            Currency=CurrencyFormatter.CreateFromCulture(Culture);
        else 
            Currency=new CurrencyFormatter(Culture, prefix, suffix);
    }

    public GridConfiguration<Student> Build()
    {
        return new GridConfiguration<Student>()
            .RowNumber(false)
            .Where(s=> s.AccountBalance >= 200)
            .OrderBy(s=> s.DateOfBirth, desc:true)  //cel mai tanar primul  
            .AddColumn("First Name", s => s.FirstName)
            .AddColumn("Last Name", s => s.LastName)
            .AddColumn("Date of Birth", s => s.DateOfBirth, cellFormatter: dt => dt.ToString("MMMM yyyy"))
            .AddColumn("Age", s => s.LongAge())
            .AddColumn("Account Balance", s => s.AccountBalance,
                //formatare custom pt afisarea banilor in fct de cultura
               cellFormatter:b => Currency.FormatCurrency(b))
            .AddColumn("Above 400", s => s.AccountBalance,alignment:Alignment.Center, cellFormatter: b => b > 400 ? "Y" : "N");
    }
}
