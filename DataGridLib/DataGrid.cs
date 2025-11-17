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

    //paginare: incepe la 1
    private int CurrentPage;

    //coloanele 
    private List<IColumn<T>>? LastDisplayedCols;

    //cel putin o coloana si fiecare un header valid 
    public DataGrid(GridConfiguration<T> Configuration, GridDataSource<T> DataSource)
    {
        this.Configuration = Configuration;
        this.DataSource = DataSource;
        Configuration.Validate();
        CurrentPage = 1; //default
    }
    private bool PagOn()
    {
        return Configuration.PageSize > 0;
    }

    //ia toate itemele, aplica filtru si ordonare
    private IEnumerable<T> OrderedItems()
    {
        //iau randurile din datasource si aplic config( filtru, ordonare)
        return DataSource.GetData(Configuration);
    }

    //total pagini
    private int TotalPages(int itemCount)
    {
        if (!PagOn())
        {
            return 1;
        }
        // pagini: (nr total iteme + pag size -1 ) / pag size
        return (itemCount + Configuration.PageSize - 1) / Configuration.PageSize;
    }

    // ia items si returneaza doar cele din pagina curenta
    private IEnumerable<T> PageSlice(IEnumerable<T> items)
    {
        if (!PagOn())
        {
            return items;
        }
        int count= items.Count();
        int totalPages = TotalPages(count);

        if (CurrentPage > totalPages)
        {
            CurrentPage = totalPages;
        }
        //cate sar se: (pagina curenta -1) * pag size
        //de ex pagina 2, pag size 10: (2-1)*10=10 sar 10 iteme
        int skip = (CurrentPage - 1) * Configuration.PageSize;
        return items.Skip(skip).Take(Configuration.PageSize);
    }

    //navigation:
    public void FirstPage()
    {
        if(PagOn())
            CurrentPage = 1;
    }
    public void LastPage()
    {
        if(PagOn())
        {
            //setez pagina curenta la ultima pagina
            CurrentPage = TotalPages(OrderedItems().Count());
        }
    }
    
    public void NextPage()
    {
        //daca e paginare si nu sunt la ultima pagina
        int totalPages = TotalPages(OrderedItems().Count());
        if (PagOn() && CurrentPage < totalPages)
        {
            CurrentPage++;
        }
    }

    public void PreviousPage()
    {
        //daca e paginare si nu sunt la prima pagina
        if (PagOn() && CurrentPage > 1)
        {
            CurrentPage--;
        }
    }

    public void GoToPage(int pageNumber)
    {
        int totalPages = TotalPages(OrderedItems().Count());

        if (PagOn())
        {
            //daca e valida, setez pagina curenta
            if (pageNumber > 0 && pageNumber <= totalPages)
            {
                CurrentPage = pageNumber;
            }
            else
            {
                throw new ArgumentException("Invalid page number.");
            }
        }
    }
    //dupa ce am creat grila, modific marimea paginii fara sa creez un datagrid nou 
    public void ChangePageSize(int pageSize)
    {
        Configuration.EnablePagination(pageSize);
        CurrentPage = 1; //resetez la prima pagina
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
        //coloanele selectate
        LastDisplayedCols = cols;

        Display(cols);
    }

    //primeste lista de coloane de afisat
    private void Display(List<IColumn<T>> cols)
    {
        //pt fiecare item din items aplica,in ordine: pt # ia s.studentid si conv la string , si le pune intr un string[] si construieste un row
        //row[0] = "1", "danie","yes"... 

        //iau itemele ordonate si filtrate
        IEnumerable<T> ordered = OrderedItems();
        //numar total iteme dupa filtru
        int totalItems = ordered.Count();

        //paginare, ia doar itemele din pagina curenta
        IEnumerable<T> pageItems = PageSlice(ordered);



        //slice ul curent devine row
        //page items= cati am pe o pagina 
        List<Row> rows = DataSource.ToRows(cols, pageItems);

        if (PagOn())
        {
            Console.WriteLine($"-- Page {CurrentPage}/{TotalPages(totalItems)} | PageSize={Configuration.PageSize} | ItemsOnPage={rows.Count} | TotalItems={totalItems} --");
        }


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

    //doar pagina curenta
    public void ExportDataGrid(IGridExporter exporter)
    {
        if (exporter == null) 
            throw new ArgumentNullException(nameof(exporter));

        string path = Path.Combine(AppContext.BaseDirectory, $"std_export.{exporter.Extension}");

        //aplic where/orderby din config
        var ordered = OrderedItems().ToList();

        //abia dupa ordonare aplic paginarea
        var pageItems = PageSlice(ordered);

        int totalItems = ordered.Count;

        //daca am afisat anterior doar anumite coloane, le folosesc pe alea
        var visibleCol = LastDisplayedCols ?? Configuration.Columns;

        var (headers, rows) = GridExportData.BuildFromItems(Configuration, DataSource,pageItems, visibleCol);

        GridPage? exportPage = BuildPageExp(totalItems, rows.Count);

        try
        {
            exporter.Export(headers, rows, path, exportPage);
            Console.WriteLine($"Exported: {path}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error exporting data grid: {ex.Message}");
        }
    }

    //daca paginarea e dezactivata returnez null, nu afiseaza nimic despre paginare
    private GridPage? BuildPageExp(int totalItems, int itemsOnPage)
    {
        if (!PagOn())
            return null;

        int totalPages = TotalPages(totalItems);

        if (CurrentPage > totalPages) 
            CurrentPage = totalPages;

        return new GridPage(
            CurrentPage,
            totalPages,
            Configuration.PageSize,
            totalItems,
            itemsOnPage);
    }
}