using DataGridLib.Export.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib.Export;

public class AsciiExporter : IGridExporter
{
    public string Extension => "txt";

    public void Export(IReadOnlyList<string> headers, IReadOnlyList<string[]> rows, string filePath, GridPage? pageExp = null)
    {
        // calc widths
        int colCount = headers.Count;
        int[] widths = new int[colCount];

        for (int i = 0; i < colCount; i++)
        {
            int maxCell = rows.Count == 0 ? 0 : rows.Max(r => r[i].Length);
            widths[i] = Math.Max(headers[i].Length, maxCell);
        }

        using var w = new StreamWriter(filePath);

        // page info
        if (pageExp != null)
        {
            GridPage c = pageExp;
            w.WriteLine($"Page {c.CurrentPage}/{c.TotalPages} | PageSize={c.PageSize} | ItemsOnPage={rows.Count} | TotalItems={c.TotalItems}");
            w.WriteLine();
        }

        // header
        for (int i = 0; i < colCount; i++)
        {
            w.Write(headers[i].PadRight(widths[i]));
            if (i < colCount - 1) 
                w.Write(" | ");
        }
        w.WriteLine();

        // separator
        int total = widths.Sum() + (colCount - 1) * 3;
        w.WriteLine(new string('-', total));

        // rows
        foreach (var row in rows)
        {
            for (int i = 0; i < colCount; i++)
            {
                w.Write(row[i].PadRight(widths[i]));

                if (i < colCount - 1) 
                    w.Write(" | ");
            }
            w.WriteLine();
        }
    }

}
