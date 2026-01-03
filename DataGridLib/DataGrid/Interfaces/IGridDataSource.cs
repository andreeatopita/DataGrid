using DataGridLib.DataGrid;

namespace DataGridLib.DataGrid.Interfaces;

public interface IGridDataSource<T>
{
    Task<IEnumerable<T>> GetDataAsync(GridConfiguration<T>? config);
    Task<List<Row>> ToRowsAsync(List<IColumn<T>> columns, GridConfiguration<T>? configuration = null);
}