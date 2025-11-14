namespace DataGridLib.Interfaces;

public interface IGridDataSource<T>
{
    IEnumerable<T> GetData(GridConfiguration<T>? config);
    List<Row> ToRows(List<IColumn<T>> columns, GridConfiguration<T>? configuration = null);
}