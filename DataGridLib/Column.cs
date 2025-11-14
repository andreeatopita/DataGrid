using DataGridLib.Interfaces;

namespace DataGridLib;

//o coloana din grid pentru un tip de rand T si o proprietate TProp ( student si nume, varsta)
public class Column<T, TProp> : IColumn<T>
{
    public string Header { get; }
    public Alignment Alignment { get; }

    //pentru a obtine valoarea proprietatii din obiectul T, de tip TProp
    private Func<T, TProp> ValueGetter { get; }
    //dintr un student, extrag numele sau varsta

    //a primit tprop datetime,decimal, il transf in text pentru afisare
    private Func<TProp,string> CellFormatter { get; }

    public Column(string header, Func<T, TProp> valueGetter, Alignment? alignment = null, Func<TProp, string>? cellFormatter = null)
    {
        Header = header;
        Alignment = alignment ?? GetDefaultAlignment(typeof(TProp));
        ValueGetter = valueGetter;
        CellFormatter = cellFormatter ?? DefaultCellFormatter;
    }

    private static Alignment GetDefaultAlignment(Type t)
    {
        if (t == typeof(int) || t == typeof(decimal) || t == typeof(DateTime) ||t==typeof(DateOnly)|| t==typeof(double) || t==typeof(float))
            return Alignment.Right;
        if (t == typeof(bool))
            return Alignment.Center;
        return Alignment.Left;
    }
    private static string DefaultCellFormatter(TProp value)
    {
        if(value == null)
            return string.Empty;

        if(value is DateOnly d)
            return d.ToString("yyyy-MM-dd");

        if(value is bool b)
            return b ? "Yes" : "No";

        if (value is DateTime dt)
            return dt.ToString("yyyy-MM-dd HH:mm"); 

        if(value is decimal dec)
            return $"{dec:C}"; 

        return value.ToString() ?? string.Empty;
    }

    // metoda pentru a obtine textul care va fi afisat in celula pentru un obiect de tip T
    public string GetCellText(T item) 
    { 
        //rezultatul are tip TProp, deci trebuie sa il convertesc in string 
        //ruleaza functia delegate pentru a extrage valoarea din obiectul item
        TProp value = ValueGetter(item); 
        //Console.WriteLine($"{Header} -> {value}"); 
        //convertesc valoarea in string, daca e null returnez string gol
        return CellFormatter(value);
    } 
    //pentru un student, extrage numele si il returneaza ca string
}
