using DataGridLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib;

public class Column<T, TProp> : IColumn<T>
{
    public string Header { get; }
    public GridDataType DataType { get; }
    public Alignment Alignment { get; }

    // functie pentru a obtine valoarea proprietatii din obiectul T, de tip TProp
    private Func<T, TProp> ValueGetter { get; }


    public Column(string header, GridDataType dataType, Func<T, TProp> valueGetter, Alignment? alignment = null)
    {
        Header = header;
        DataType = dataType;
        Alignment = alignment ?? GetDefaultAlignment(dataType);
        ValueGetter = valueGetter;
    }

    public static Alignment GetDefaultAlignment(GridDataType dataType)
    {
        return dataType switch
        {
            GridDataType.Int => Alignment.Right,
            GridDataType.Decimal => Alignment.Right,
            GridDataType.DateTime => Alignment.Right,
            GridDataType.String => Alignment.Left,
            GridDataType.Bool => Alignment.Center,
            _ => Alignment.Left
        };
    }

    // metoda pentru a obtine textul care va fi afisat in celula pentru un obiect de tip T
    public string GetCellText(T item)
    {
        var value = ValueGetter(item);

        // convertesc valoarea la string, gestionez cazul null
        return value?.ToString() ?? string.Empty;
    }

}
