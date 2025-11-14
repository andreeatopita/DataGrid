using DataGridLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataGrid_1.Formatting;


namespace DataGrid_1.Grids;

//aici sa pun conditii la where order by
public class InactiveStudentsGrid : IGridBuilder<Student>
{
    public GridConfiguration<Student> Build()
    {
        return new GridConfiguration<Student>()
            .RowNumber(true)
            .Where(s => !s.IsActive)
            .OrderBy(s => s.LastActiveAt, asc: true)
            .AddColumn("Name", s => s.NameWithFather())
            .AddColumn("Age", s => s.ShortAge())
            .AddColumn("Last Active", s => s.LastActiveAt, cellFormatter: dt => dt.ToString("yyyy-MM-dd hh:mm tt"));
    }
}
