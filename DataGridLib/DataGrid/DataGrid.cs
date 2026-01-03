using DataGrid_1.Export;
using DataGridLib.DataGrid.Interfaces;
using DataGridLib.Export;
using DataGridLib.Export.Interfaces;

namespace DataGridLib.DataGrid;

//orchestrator care ia datele, aplica config( coloane, filtru, numerotare randuri) 
public class DataGrid<T>
{
    private GridConfiguration<T> Configuration { get; }
    private GridDataSource<T> DataSource { get; }
    private PageNavigation Nav { get; }

    //coloanele 
    private List<IColumn<T>>? LastDisplayedCols;

    //cel putin o coloana si fiecare un header valid 
    public DataGrid(GridConfiguration<T> Configuration, GridDataSource<T> DataSource)
    {
        this.Configuration = Configuration;
        this.DataSource = DataSource;
        Configuration.Validate();

        //porneste de la pagina1
        //transmit acel func care citeste din config valoarea actuala a pag size
        Nav = new PageNavigation(() => Configuration.PageSize);
    }
    private async Task<IEnumerable<T>> OrderedItemsAsync() => await DataSource.GetDataAsync(Configuration);
     //iau randurile din datasource si aplic config( filtru, ordonare)

    //pagina activata si setare la 1
    public void EnablePagination(int pageSize) => Configuration.EnablePagination(pageSize);
    //dazactivez
    public void DisablePagination() => Configuration.DisablePagination();
    //dau enable si dupa setez la pagina pe care o zic
    public void ChangePageSize(int pageSize) => EnablePagination(pageSize);
    public void First() => Nav.FirstPage();
    public void Last() => Nav.LastPage();
    public void Next() => Nav.NextPage();
    public void Previous() => Nav.PreviousPage();
    public void GoToPage(int pageNumber) => Nav.GoToPageNo(pageNumber);

    //overload: afisez doar coloanele a caror header le dau eu ca parametru
    public async Task DisplayAsync(params string[] columnHeaders)
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
            //pastrez doar coloanele ale caror headere se regasesc in columnheaders
            //face comparatie case insensitive

            if (cols.Count == 0)
                cols = Configuration.Columns;
        }
        //coloanele selectate
        LastDisplayedCols = cols;

        await RenderAsync(cols);
    }

    private async Task RenderAsync(List<IColumn<T>> cols)
    {
        //pt fiecare item din items aplica,in ordine: pt # ia s.studentid si conv la string , si le pune intr un string[] si construieste un row
        //row[0] = "1", "danie","yes"... 

        //iau itemele ordonate si filtrate
        IEnumerable<T> ordered = (await OrderedItemsAsync()).ToList();

        //aici setez numarul total de iteme pt paginare
        Nav.UpdateTotalItems(ordered.Count());

        var pageItems=Nav.PageSlice(ordered);

        int totalItems = ordered.Count();
        //slice ul curent devine row
        //page items= cati am pe o pagina 

        //items in row( sync)
        List<Row> rows = DataSource.ToRows(cols, pageItems);

        if (Nav.Enabled)
        {
            Console.WriteLine($"-- Page {Nav.CurrentPage}/{Nav.TotalPages} | PageSize={Nav.PageSize} | ItemsOnPage={rows.Count} | TotalItems={totalItems} --");
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
    public async Task ExportDataGridAsync(IGridExporter exporter)
    {
        if (exporter == null) 
            throw new ArgumentNullException("Exporter null.");


        string path = Path.Combine(AppContext.BaseDirectory, $"std_export.{exporter.Extension}");

        //aplic where/orderby din config
        //iau datele ordonate async
        var ordered = (await OrderedItemsAsync()).ToList();

        //abia dupa ordonare aplic paginarea
        var pageItems = Nav.PageSlice(ordered);

        int totalItems = ordered.Count;

        //daca am afisat anterior doar anumite coloane, le folosesc pe alea
        var visibleCol = LastDisplayedCols ?? Configuration.Columns;

        //sync, lucreaza cu obiecte in memorie
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
        if (!Nav.Enabled)
            return null;

        int totalPages = Nav.TotalPages;

        return new GridPage(
            Nav.CurrentPage,
            totalPages,
            Nav.PageSize,
            totalItems,
            itemsOnPage);
    }
}