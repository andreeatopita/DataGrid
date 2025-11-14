using DataGridLib.Interfaces;


namespace DataGridLib;

public class Row : IRow
{
    //cells = valorile dintr un rand de tabel, vector de stringuri
    public string[] Cells { get; private set; }

    //this = face referire la instanta curenta a clasei, indexer pt a accesa valorile din cells
    //pentru a putea scrie row[0], row[1] etc, nu row.Cells[0]
    public string this[int index] => Cells[index];

    public int Count => Cells.Length;

    //constructor care primeste o colectie de stringuri (celulele din rand) 
    public Row(IEnumerable<string> cells)
    {
        //convertesc colectia in array si o atribui proprietatii cells; indexer this[int] mai rapid 
        Cells = cells.ToArray();
    }

    //initializare cells cu vector gol
    public Row()
    {
        // sau cu vector gol : cells = new string[0];
        Cells = Array.Empty<string>();
    }
}

