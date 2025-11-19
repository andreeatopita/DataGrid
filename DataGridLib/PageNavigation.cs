using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib;

internal class PageNavigation
{
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    

    public bool Enabled => PageSize > 0;

    //numarul total de iteme folosit la calculul paginilor
    private int totalItems;
    
    public int TotalPages => TotalPagesMeth(totalItems);

    //calculez total pagini din total items
    public int TotalPagesMeth(int totalItems)
    {
        if (!Enabled)
        {
            return 1;
        }
        // pagini: (nr total iteme + pag size -1 ) / pag size
        return (totalItems + PageSize - 1) / PageSize;
    }


    public PageNavigation(int pageSize)
    {
        SetPageSize(pageSize);
    }

    public void SetPageSize(int pageSize)
    {
        if (pageSize < 0)
        {
            throw new ArgumentException("Page size must be greater than zero.");
        }

        PageSize = pageSize;
        CurrentPage = 1; //reset to first page
    }

    //metoda care actualizeaza total items, adica cate iteme sunt in total
    public void UpdateTotalItems(int totalItems)
    {
        if(totalItems < 0)
        {
            throw new ArgumentException("Total items cannot be negative.");
        }

        this.totalItems = totalItems;

        //daca pagina curenta e mai mare decat total pagini, setez pagina curenta la ultima pagina
        if (CurrentPage > TotalPages)
        {
            CurrentPage = TotalPages;
        }

        if(CurrentPage < 1)
        {
            CurrentPage = 1;
        }
    }

    public IEnumerable<T> PageSlice<T>(IEnumerable<T> items)
    {
        if (!Enabled)
        {
            return items;
        }
  
        //cate sar se: (pagina curenta -1) * pag size
        //de ex pagina 2, pag size 10: (2-1)*10=10 sar 10 iteme
        int skip = (CurrentPage - 1) * PageSize;
        return items.Skip(skip).Take(PageSize);
    }

    //navigation:
    public void FirstPage()
    {
        if (Enabled)
            CurrentPage = 1;
    }


    public void LastPage()
    {
        if (Enabled)
        {
            //setez pagina curenta la ultima pagina
            CurrentPage = TotalPages;
        }
    }

    public void NextPage()
    {
        if (Enabled && CurrentPage < TotalPages)
        {
            CurrentPage++;
        }
        else
        {
            Console.WriteLine("\nOn the last page.\n");
        }
    }


    public void PreviousPage()
    {
        //daca e paginare si nu sunt la prima pagina
        if (Enabled && CurrentPage > 1)
        {
            CurrentPage--;
        }
        else
        {
            Console.WriteLine("\nOn the first page.\n");
        }
    }

    public void GoToPageNo(int pageNumber)
    {
        if (Enabled)
        {
            //daca e valida, setez pagina curenta
            if (pageNumber > 0 && pageNumber <= TotalPages)
            {
                CurrentPage = pageNumber;
            }
            else
            {
                CurrentPage = 1;
                Console.WriteLine("\nInvalid page number. Going to first page.\n");
            }
        }
    }
}
