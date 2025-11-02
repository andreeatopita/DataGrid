using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataGridLib.Interfaces;



namespace DataGridLib;

public class Row : IRow
{
    //cells = valorile dintr un rand de tabel, vector de stringuri
    public string[] Cells { get; private set; }
    //this = face referire la instanta curenta a clasei, indexer pt a accesa valorile din cells
    public string this[int index] => Cells[index];


    //nr de celule din rand 
    public int Count => Cells.Length;

    //constructor care primeste o colectie de stringuri (celulele din rand) 
    //IEnumerable ca sa nu depind de sursa lui mai tarziu 
    public Row(IEnumerable<string> cells)
    {
        //convertesc colectia in array si o atribui proprietatii cells; indexer this[int] mai rapid 
        //o singura enumerare
        this.Cells = cells.ToArray();
    }

    //initializare cells cu vector gol
    public Row()
    {
        // sau cu vector gol : cells = new string[0];
        Cells = Array.Empty<string>();
    }
}

