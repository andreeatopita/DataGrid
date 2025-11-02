namespace DataGridLib.Interfaces;

public interface IGridConfiguration<T>
{
    List<IColumn<T>> Columns { get; }
    Alignment RowNumberAligment { get; }
    bool ShowRowNumber { get; }
    GridConfiguration<T> AddColumn<TProp>(string header, GridDataType dataType, Func<T, TProp> valueGetter, Alignment? alignment = null);
    GridConfiguration<T> OrderBy<TKey>(Func<T, TKey> keySelector, bool desc = false);
    GridConfiguration<T> ResetQuery();
    GridConfiguration<T> RowNumber(bool show = true, Alignment alignment = Alignment.Right);
    GridConfiguration<T> Skip(int n);
    GridConfiguration<T> Take(int n);
    void Validate();
    GridConfiguration<T> Where(Func<T, bool> predicate);
}