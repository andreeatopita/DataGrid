using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib.DataGrid.Interfaces;

public interface IRow
{
    int Count { get; }

    //this - indexer, adica pot accesa elementele dintr un obiect de tip IRow folosind un index
    //vector de stringuri, fiecare element reprezinta o celula din rand
    string this[int index] { get; }
}
