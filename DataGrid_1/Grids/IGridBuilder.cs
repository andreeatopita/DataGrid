using DataGridLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Grids;

public interface IGridBuilder<T>
{
    GridConfiguration<T> Build();
}
