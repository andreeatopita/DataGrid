using DataGridLib.Contracts;
using DataGridLib.DataGrid.Interfaces;

namespace DataGridLib.DataGrid;

public class GridDataSource<T> : IGridDataSource<T>
{
    //nu lista=> trb repository
    //DataSource<IRepository<T>> in loc de IEnumerable<T>
    //Datagrid<cconfig, Irepository<T>> : unde irepository e JsonRepository, SqlRepository etc

    //problema: 
    //1 -> t = student/inactive student/ etc = alte tipuri de obiecte
    //aici fac partea asta in care nu extrag toate datele din BD, ci doar ce imi trebuie
    // cred? - aici in aplicatie acel query(apply) ca sa extreg doar ce imi trebuie

    //2-> fiecare repository in parte are si logica de " ce date am nevoie" -> JsonRepositoryStudentInactive , cred?
    //adica fiecare repository implementeaza logica de filtrare a datelor, cred, acel query integrat in el

    //ambele au nevoie de obiecte si dto : problema e unde aplic logica de filtrare a datelor


    // aici ar trebui sa arate asa constructorul
    //public gridatasource(irepository<t> repo) , si data source alege SURSA DATELOR 
    //filtrarea datelor practic ar trebui sa ramana aici, cum am mai jos 

    //am un datagrid cu configuratie si sursa datelor, pe configuratie aplic de exemplu Where, si aici in Datasource aplic si obtin datele filtrate

    private IRepository<T> Repository { get; }

    //constructor care primeste o colectie de elemente de tip T, le atribuie proprietatii Data
    public GridDataSource(IRepository<T> repository)
    {
        Repository = repository;
    }


    //iau datele, aplic config ul de grid(where, orderby)
    public async Task<IEnumerable<T>> GetDataAsync(GridConfiguration<T>? config)
    {
        //load din repository: json, sql, api
        IReadOnlyList<T> data = await Repository.LoadAsync();

        //daca nu am config, returnez datele fara modificari
        if (config == null)
            return data;
        else
            return config.Apply(data);
    }

    public async Task<List<Row>> ToRowsAsync(List<IColumn<T>> columns, GridConfiguration<T>? configuration = null)
    {
        //columns- coloanele care definesc structura gridului, nume, varsta
        //configuration- contine informatii despre coloane, filtru, ordonare

        IEnumerable<T> items = await GetDataAsync(configuration); //transformari

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
