using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib;

//ca sa nu folosesc typeof(...) , am facut un enum pt tipurile de date, aliniera implicita
public enum GridDataType
{
    Int,
    Decimal,
    String,
    DateTime,
    Bool
}

//enum pentru alinierea textului in coloana, cum sa asez textul in celula
public enum Alignment
{
    Left,
    Center,
    Right
}