using DataGridLib.Interfaces;


namespace DataGridLib;

//
public class GridDataSource<T> : IGridDataSource<T>
{
    //lista de date, IEnumerable pentru a putea folosi orice colectie 
    private IEnumerable<T> Data { get; }

    //constructor care primeste o colectie de elemente de tip T, le atribuie proprietatii Data
    public GridDataSource(IEnumerable<T> data)
    {
        Data = data;
    }

    //pentru a obtine datele nemodificate

    public IEnumerable<T> GetData(GridConfiguration<T>? config)
    {
        //daca nu am config, returnez datele fara modificari
        if (config == null)
            return Data;
        else
            return config.Apply(Data);
    }

    //transf elemente filtrare in randuri pentru afisare in grid
    public List<Row> ToRows( List<IColumn<T>> columns, GridConfiguration<T>? configuration = null)
    {
        //columns- coloanele care definesc structura gridului, nume, varsta
        //configuration- contine informatii despre coloane, filtru, ordonare

        IEnumerable<T> items = GetData(configuration); //transformari

        List<Row> rows = new List<Row>();

        //pt fiecare student din lista de studenti
        foreach (T item in items)
        {
            //parcurg toate coloanele si extrag textul pentru celula corespunzatoare din randul curent
            IEnumerable<string> cells = columns.Select(c => c.GetCellText(item));

            //creez un row din acele celule si adaug la rezultat
            rows.Add(new Row(cells));
        }
        return rows;
    }

    //transf elemente filtrare in randuri pentru afisare in grid
    public List<Row> ToRows(List<IColumn<T>> columns, IEnumerable<T> items)
    {
        var rows = new List<Row>();
        foreach (var item in items)
            rows.Add(new Row(columns.Select(c => c.GetCellText(item))));
        return rows;
    }
}
