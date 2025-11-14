using DataGridLib.Export;
using DataGridLib.Export.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Export;

//factory pt creare
public class GridExporterFactory : IGridExporterFactory
{
    public IGridExporter? CreateExporter(string? extension)
    {
        
        string? input= extension?.Trim().ToLower();

        return input switch
        {
            "1" or "txt"  => new AsciiExporter(),
            "2" or "csv" => new CsvExporter(),
            "3" or "xml" => new XmlExporter(),
            null or "ascii" =>null,
            _ => null  //console
        };
    }
}
