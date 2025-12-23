using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib.Contracts;

public interface IRepository<T>
{
    Task<IReadOnlyList<T>> LoadAsync();
}
