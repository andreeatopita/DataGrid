using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataGrid_1.Repository;

public interface IRepository<T>
{
    Task<IReadOnlyList<T>> LoadAsync();
    //Task SaveAsync(IEnumerable<T> items);

}
