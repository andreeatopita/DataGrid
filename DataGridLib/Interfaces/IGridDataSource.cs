namespace DataGridLib.Interfaces;

public interface IGridDataSource<T>
{
    IEnumerable<T> GetData();
    List<Row> ToRows(List<IColumn<T>> columns);
    List<Row> ToRows(List<IColumn<T>> columns, IEnumerable<T> items);
}