using DataGridLib.Export.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib.Export;

public class CsvExporter : IGridExporter
{
    public string Extension => "csv";

    public void Export(IReadOnlyList<string> headers, IReadOnlyList<string[]> rows, string filePath, GridPage? pageExp = null)
    {
        try
        {
            //stream writer pt scriere in fisier
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

            if (pageExp != null)
            {
                var c = pageExp;
                writer.WriteLine($"# Page {c.CurrentPage}/{c.TotalPages} | PageSize={c.PageSize} | ItemsOnPage={rows.Count} | TotalItems={c.TotalItems}");
            }


            //scriu headerele cu , 
            writer.WriteLine(string.Join(",", headers.Select(NormalizeHeader)));

            //fiecare rand
            foreach (var row in rows)
            {
                writer.WriteLine(string.Join(",", row.Select(NormalizeHeader)));
            }
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine($"Cannot write CSV {filePath}: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.Error.WriteLine($"Access denied for {filePath}: {ex.Message}");
        }
    }

    private static string NormalizeHeader(string value)
    {
        if (value == null)
            value= string.Empty;


        //reguli csv: " " inceput/sf de camp, daca apar in interior se dubleaza
        if(value.Contains('"') || value.Contains(',') || value.Contains('\n'))
        {
            //daca contine ghilimele, virgule, newline, o incadrez in ghilimele si dublez ghilimelele din interior
            value = "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        return value;
    }
}
