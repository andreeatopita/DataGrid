using DataGridLib.Interfaces;

namespace DataGridLib;


public class DataGrid<T>
{
    //readonly ca sa le pot atribui doar in constructor , sau la initializatorul de proprietate
    private GridConfiguration<T> Configuration { get; }
    private GridDataSource<T> DataSource { get; }


    public DataGrid(GridConfiguration<T> Configuration, GridDataSource<T> DataSource)
    {
        this.Configuration = Configuration;
        this.DataSource = DataSource;

        //validare 
        Configuration.Validate();
    }

    //OVERLOAD LA METODA DE AFISARE!

    //afisez toate coloanele din config
    public void Display()
    {
        Display(Configuration.Columns);
    }

    //overload: afisez doar coloanele a caror header le dau eu ca parametru
    public void Display(params string[] columnHeaders)
    {
        //filtrez coloanele din config dupa headerele date ca parametru
        List<IColumn<T>> cols;

        if (columnHeaders == null || columnHeaders.Length == 0)
        {
            cols = Configuration.Columns;
        }
        else
        {
            cols = Configuration.Columns
                .Where(c => columnHeaders.Contains(c.Header, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (cols.Count == 0)
                cols = Configuration.Columns; // fallback
        }

        Display(cols);
    }

    private void Display(List<IColumn<T>> cols)
    {
        // aplica Where/OrderBy/Skip/Take apoi mapeaza la row
        var items = Configuration.Apply(DataSource.GetData()).ToList();
        var rows = DataSource.ToRows(cols.ToList(), items);

        //calcul latimi pt coloanele 
        var widths = new int[cols.Count];
        for (int c = 0; c < cols.Count; c++)
            widths[c] = Math.Max(cols[c].Header.Length,
                                 rows.Select(r => r[c].Length).DefaultIfEmpty(0).Max());

        // row count ca coloana (daca e activat)
        int rowNumWidth = 0;
        if (Configuration.ShowRowNumber)
            rowNumWidth = Math.Max("No.".Length, rows.Count.ToString().Length);

        // header 
        if (Configuration.ShowRowNumber)
            Console.Write(Align("No.", rowNumWidth, Configuration.RowNumberAligment) + " | ");

        for (int c = 0; c < cols.Count; c++)
        {
            Console.Write(Align(cols[c].Header, widths[c], Alignment.Left));
            if (c < cols.Count - 1) Console.Write(" | ");
        }
        Console.WriteLine();

        // separator
        int totalWidth = widths.Sum() + Math.Max(0, cols.Count - 1) * 3;
        if (Configuration.ShowRowNumber) 
            totalWidth += rowNumWidth + 3; // "No." + " | "

        Console.WriteLine(new string('-', totalWidth));

        //randuri 
        for (int r = 0; r < rows.Count; r++)
        {
            if (Configuration.ShowRowNumber)
                Console.Write(Align((r + 1).ToString(), rowNumWidth, Configuration.RowNumberAligment) + " | ");

            for (int c = 0; c < cols.Count; c++)
            {
                Console.Write(Align(rows[r][c], widths[c], cols[c].Alignment));
                if (c < cols.Count - 1) Console.Write(" | ");
            }
            Console.WriteLine();
        }
    }

    private static string Align(string text, int width, Alignment align)
    {
        text ??= string.Empty;
        if (text.Length > width) return text[..width];
        int pad = width - text.Length;
        return align switch
        {
            Alignment.Left => text + new string(' ', pad),
            Alignment.Right => new string(' ', pad) + text,
            Alignment.Center => new string(' ', pad / 2) + text + new string(' ', pad - pad / 2),
            _ => text
        };
    }

}


/*
 * 
 * old implementation without generics
     public void Display()
    {
        var rows = dataSource.ToRows(configuration.Columns);

        var widths = new int[configuration.Columns.Count];
        for (int c = 0; c < configuration.Columns.Count; c++)
            widths[c] = Math.Max(configuration.Columns[c].Header.Length,
                                 rows.Select(r => r[c].Length).DefaultIfEmpty(0).Max());

        //optional row number
        int rowNumWidth = 0;
        if( configuration.ShowRowNumber )
        {
            rowNumWidth = Math.Max("No".Length, rows.Count.ToString().Length);
        }


        // header
        if (configuration.ShowRowNumber)
            Console.Write(Align("#", rowNumWidth, configuration.RowNumberAligment) + " | ");

        for (int c = 0; c < configuration.Columns.Count; c++)
        {
            Console.Write(Align(configuration.Columns[c].Header, widths[c], Alignment.Left));
            if (c < configuration.Columns.Count - 1) Console.Write(" | ");
        }
        Console.WriteLine();

        // (3) SEPARATOR (include No. + " | ")
        int totalWidth = widths.Sum() + Math.Max(0, configuration.Columns.Count - 1) * 3;
        if (configuration.ShowRowNumber) totalWidth += rowNumWidth + 3;
        Console.WriteLine(new string('-', totalWidth));

        // (4) ROWS
        for (int r = 0; r < rows.Count; r++)
        {
            if (configuration.ShowRowNumber)
                Console.Write(Align((r + 1).ToString(), rowNumWidth, configuration.RowNumberAligment) + " | ");

            for (int c = 0; c < configuration.Columns.Count; c++)
            {
                var col = configuration.Columns[c];
                Console.Write(Align(rows[r][c], widths[c], col.Alignment));
                if (c < configuration.Columns.Count - 1) Console.Write(" | ");
            }
            Console.WriteLine();
        }
    }
 */





/*
public List<Column> Columns { get; }
public List<Row> Rows { get; }

public DataGrid(List<Column> columns, List<Row> rows)
{
    Columns = columns;
    Rows = rows;
}

public void AddColumn(string header, string field)
{
    Columns.Add(new Column(header, field));
}
public void AddRow(Row row)
{
    Rows.Add(row);
}


public void Display()
{
    var columns = Columns;
    var rows = Rows;

    //latimea fiecarei coloane
    int[] maxWidths = new int[columns.Count];

    for (int i = 0; i < columns.Count; i++)
    {
        maxWidths[i] = columns[i].Header.Length;

        foreach (Row r in rows)
        {
            //trygetvalue - key - value , out tvalue value - true daca contine el cu cheia specificata
            // r.rows.TryGetValue(columns[i].Field, out object value);
            var value = r.GetValue(columns[i].Field);
            string text = value?.ToString() ?? string.Empty;
            if (text.Length > maxWidths[i])
                maxWidths[i] = text.Length;
            //iau valoarea asociata campului field(cheia, d eex full name) , transform in text si daca e mai lung decat header ul actual, maresc latimea 
        }
    }

    //header
    for (int i = 0; i < columns.Count; i++)
    {
        Console.Write(columns[i].Header.PadRight(maxWidths[i]) + " | ");
    }
    Console.WriteLine();
    //padright - completeaza textul cu spatii pana la lungimea n 
    //name.padright(10) -> name cu 10 spatii la dreapta 

    //linie sub header
    Console.WriteLine(new string('-', maxWidths.Sum() + (columns.Count * 3)));
    //maxWidths - totalul caracterelor de date
    //columns.count * 3 - spatii si barele | 


    //fiecara rand parcurg coloanele in ordinea lor si afisez valoarea asociata ( field din column ) 
    foreach (Row row in rows)
    {
        //pt fiecare rand, pt fiecare coloana iau valoarea asociala field ului din dict
        //
        for (int i = 0; i < columns.Count; i++)
        {
            var value = row.GetValue(columns[i].Field);
            string text = value?.ToString() ?? string.Empty;
            Console.Write(text.PadRight(maxWidths[i]) + " | ");
        }
        Console.WriteLine();
    }
}
*/
