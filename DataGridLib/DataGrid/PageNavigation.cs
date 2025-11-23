using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib.DataGrid;

internal class PageNavigation
{
    //sa citesc pagesize-ul din config
    private Func<int> PageSizeGetter { get; }
    //ultimul pagesize pentru detectare schimbari
    private int lastPageSize;
    //total petntru calcul pagini
    private int totalItems;    


    public int PageSize => PageSizeGetter();
    //porneste de la 1 cand paginarea e activata 
    public int CurrentPage { get; private set; } = 1;
    public bool Enabled => PageSize > 0;

    public int TotalPages => TotalPagesMethod(totalItems);

    //calculez total pagini din total items
    public int TotalPagesMethod(int totalItems)
    {
        if (!Enabled)
        {
            return 1;
        }
        // pagini: (nr total iteme + pag size -1 ) / pag size
        return (totalItems + PageSize - 1) / PageSize;
    }


    public PageNavigation(Func<int> pageSizeProvider)
    {
        PageSizeGetter= pageSizeProvider;
        //valoarea curenta din config
        lastPageSize = PageSize;
    }

    //metoda care actualizeaza total items, adica cate iteme sunt in total
    public void UpdateTotalItems(int totalItems)
    {
        if(totalItems < 0)
        {
            throw new ArgumentException("Total items cannot be negative.");
        }

        this.totalItems = totalItems;

        //daca schimb page size, resetez pagina curenta la 1
        if (PageSize != lastPageSize)
        {
            lastPageSize = PageSize;
            CurrentPage = 1;
        }

        //daca pagina curenta e mai mare decat total pagini, setez pagina curenta la ultima pagina
        if (Enabled && CurrentPage > TotalPages)
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
        if (!Enabled) 
        { 
            Console.WriteLine("\nPagination is disabled.\n"); 
            return; 
        }

        
        CurrentPage = 1;
    }


    public void LastPage()
    {
        if (!Enabled)
            Console.WriteLine("\nPagination is disabled.\n");

        if (Enabled)
        {
            //setez pagina curenta la ultima pagina
            CurrentPage = TotalPages;
        }
    }

    public void NextPage()
    {
        if (!Enabled)
        {
            Console.WriteLine("\nPagination is not enabled.\n");
            return;
        }

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
        if (!Enabled)
        {
            Console.WriteLine("\nPagination is not enabled.\n");
            return;
        }

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
        if(!Enabled)
        {
            Console.WriteLine("\nPagination is not enabled.\n");
            return;
        }


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
