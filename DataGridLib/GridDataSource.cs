using DataGridLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib;

public class GridDataSource<T> : IGridDataSource<T>
{
    //lista mea de date, IEnumerable pentru a putea folosi orice colectie care implementeaza acest interface
    private IEnumerable<T> Data { get; }

    public GridDataSource(IEnumerable<T> items)
    {
        this.Data = items;
    }

    //pentru a obtine datele
    public IEnumerable<T> GetData()
    {
        return Data;
    }

    public List<Row> ToRows(List<IColumn<T>> columns, IEnumerable<T> items)
    {
        //convertesc fiecare element din items in Row folosind coloanele specificate
        var rows = new List<Row>();
        foreach (var item in items)
            rows.Add(new Row(columns.Select(c => c.GetCellText(item))));
        return rows;
    }

    public List<Row> ToRows(List<IColumn<T>> columns) => ToRows(columns, Data);

}
