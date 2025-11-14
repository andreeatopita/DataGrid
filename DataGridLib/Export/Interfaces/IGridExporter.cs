using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib.Export.Interfaces;

public interface IGridExporter
{
    string Extension { get; }
   
    void Export(IReadOnlyList<string> headers, IReadOnlyList<string[]> rows, string filePath);
}
