using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataGridLib.DataGrid;
using DataGridLib.DataGrid.Interfaces;

namespace DataGridLib.Export;

//datele pregatite pt exportm copie a headerelor si randurilor 
//antete si celule ca string pentru csv/xml
public static class GridExportData
{
    //metoda pentru export cu o lista de itemi data ca parametru
    public static (IReadOnlyList<string> headers, IReadOnlyList<string[]> rows) BuildFromItems<T>(GridConfiguration<T> config, GridDataSource<T> dataSource,IEnumerable<T> items, List<IColumn<T>> columns)
    {
        //coloanele configurate prin addcolumn
        //List<IColumn<T>> columns = config.Columns;

        //headere
        List<string> headers = new List<string>();

        //adaug # pt nr rand daca e cazul
        if (config.ShowRowNumber)
            headers.Add("#");


        //adaug numele celorlalte coloane
        headers.AddRange(columns.Select(c => c.Header));

        //datele in obiecte row( rand = string[] celule)
        List<Row> formattedRows = dataSource.ToRows(columns,items);

        //lista de randuri
        List<string[]> rows = new List<string[]>(formattedRows.Count);

        //fiecare rand din rowObjs
        for (int r = 0; r < formattedRows.Count; r++)
        {
            //pt fiecare index de coloana, ia celula corespunzatoare din randul r

            List<string> dataCells = new List<string>();
            for (int i = 0; i < columns.Count; i++)
            {
                dataCells.Add(formattedRows[r][i]);
            }

            //daca e actv coloana, adaug la inceput nr randului
            if (config.ShowRowNumber)
            {
                dataCells.Insert(0, (r + 1).ToString());
            }

            //convertesc la array si adaug la rows
            rows.Add(dataCells.ToArray());
        }

        return (headers, rows);
        //lista de randuri si string[] celule
    }
}
