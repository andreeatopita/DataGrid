using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataGridLib;
namespace DataGridLib.Interfaces;
public interface IColumn<T>
{
    string Header { get; }
    //enum pentru tipurile de date
    Alignment Alignment { get; }
    //metoda pentru a obtine textul care va fi afisat in celula pentru un obiect de tip T

    string GetCellText(T item);
}
