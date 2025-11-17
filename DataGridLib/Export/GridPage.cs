using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib.Export;

//record: date immutable pentru a retine informatii despre paginare la export
public record GridPage(
    int CurrentPage,
    int TotalPages,
    int PageSize,
    int TotalItems,
    int ItemsOnPage
);