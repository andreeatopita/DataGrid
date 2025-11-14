using DataGrid_1.Export;
using DataGridLib.Export;
using DataGridLib.Export.Interfaces;
using DataGridLib.Interfaces;

namespace DataGridLib;

//orchestrator care ia datele, aplica config( coloane, filtru, numerotare randuri) 
public class DataGrid<T>
{
    private GridConfiguration<T> Configuration { get; }
    private GridDataSource<T> DataSource { get; }

    //cel putin o coloana si fiecare un header valid 
    public DataGrid(GridConfiguration<T> Configuration, GridDataSource<T> DataSource)
    {
        this.Configuration = Configuration;
        this.DataSource = DataSource;
        Configuration.Validate();
    }


    //overload: afisez doar coloanele a caror header le dau eu ca parametru
    public void Display(params string[] columnHeaders)
    {
        //filtrez coloanele din config dupa headerele date ca parametru
        List<IColumn<T>> cols;

        if (columnHeaders == null || columnHeaders.Length == 0)
        {
            //daca nu s-au dat headere ( display() )  atunci afisez toate
            cols = Configuration.Columns;
        }
        else
        {
            //daca am lista de headere, filtrez coloanele din configuratie c asa retin doar pe cele ale caror header apare 
            cols = Configuration.Columns //iau ista completa de coloane
                .Where(c => columnHeaders.Contains(c.Header, StringComparer.OrdinalIgnoreCase))
                .ToList();
            //pastrez doar cooanele ale caror headere se regasesc in columnheaders
            //face comparatie case insesitive

            if (cols.Count == 0)
                cols = Configuration.Columns;
        }

        Display(cols);
    }

    //primeste lista de coloane de afisat
    private void Display(List<IColumn<T>> cols)
    {
        //pt fiecare item din items aplica,in ordine: pt # ia s.studentid si conv la string , si le pune intr un string[] si construieste un row
        //row[0] = "1", "danie","yes"... 
        List<Row> rows = DataSource.ToRows(cols, Configuration);

        //calcul latimi pt coloanele ,max dintre lungimea header ului si cea mai lunga val textuala 
        int[] widths = new int[cols.Count];
        for (int c = 0; c < cols.Count; c++)
            widths[c] = Math.Max(cols[c].Header.Length, rows.Select(r => r[c].Length).DefaultIfEmpty(0).Max());

        //row count ca coloana (daca e activat)
        //calculez latimea ei: maxim intre no si cel mai mare nr in caractere
        int rowNumWidth = 0;
        if (Configuration.ShowRowNumber)
            rowNumWidth = Math.Max("#".Length, rows.Count.ToString().Length);

        // header , pt coloana numerelor ( no.) aliniat cu  separator | 
        if (Configuration.ShowRowNumber)
            Console.Write(Align("#", rowNumWidth, Configuration.RowNumberAligment) + " | ");

        //afisare header coloanele,  aliniate left
        for (int c = 0; c < cols.Count; c++)
        {
            Console.Write(Align(cols[c].Header, widths[c], Alignment.Left));
            if (c < cols.Count - 1)
                Console.Write(" | ");
        }
        Console.WriteLine();

        //separator sub header, latimea totala = suma latimi coloane + spatii dintre ele
        int totalWidth = widths.Sum() + Math.Max(0, cols.Count - 1) * 3;
        if (Configuration.ShowRowNumber)
            totalWidth += rowNumWidth + 3; // "No." + " | "

        Console.WriteLine(new string('-', totalWidth));

        //r merge prin toate randurile , c prin coloanele afisate
        for (int r = 0; r < rows.Count; r++)
        {
            //daca e coloana cu nr randului, o afisez prima
            //aliniata si cu latimea calculata
            if (Configuration.ShowRowNumber)
                Console.Write(Align((r + 1).ToString(), rowNumWidth, Configuration.RowNumberAligment) + " | ");
            //r+1 pt ca indexul r incepe de la 0

            //parcurge coloanele si afiseaza celulele aliniate corespunzator
            for (int c = 0; c < cols.Count; c++)
            {
                //row[r][c] - ia textul celulei 
                //widths[c] - latimea maxima coloanei c
                //cols[c].Alignment - alinierea specificata in coloana c
                Console.Write(Align(rows[r][c], widths[c], cols[c].Alignment));

                // pun | daca nu e ultima coloana
                if (c < cols.Count - 1)
                    Console.Write(" | ");
            }
            Console.WriteLine();
        }
    }

    private static string Align(string text, int width, Alignment align)
    {
        if (text == null)
            text = string.Empty;

        int pad = width - text.Length; //spatii de adaugat ca sa ajung la latimea width

        return align switch
        {
            Alignment.Left => text + new string(' ', pad),  //text+ spatii
            Alignment.Right => new string(' ', pad) + text, //spatii + text
            Alignment.Center => new string(' ', pad / 2) + text + new string(' ', pad - pad / 2),
            //impart spatiul : jumatate inainte pad/2, text, restupa dupa pad-pad/2: centrat
            _ => text
        };
    }

    public void ExportDataGrid(IGridExporter exporter, string path)
    {
        if (exporter == null) 
            throw new ArgumentNullException(nameof(exporter));

        if (string.IsNullOrWhiteSpace(path)) 
            throw new ArgumentException("Invalid path.", nameof(path));

        var (header, row) = GridExportData.Build(Configuration, DataSource);

        exporter.Export(header, row, path);

    }
}