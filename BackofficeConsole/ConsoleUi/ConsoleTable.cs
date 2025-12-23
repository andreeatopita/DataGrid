using BackofficeConsole.ConsoleUi;
using Cocona.Command;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackofficeConsole.ConsoleUi;

//clasa pentru afisare tabele in consola
//static ca sa pot apela metodele fara sa instantez clasa
public static class ConsoleTable
{
    //intoarce string , textul care va fi afisat in consola
    public static void Print<T>(IEnumerable<T> items, params (string Header, Func<T, string> Selector, Align Align)[] columns)
    {
        //selector: delegate care va primit obiect de tip T(adica dto urile mele) si intoarce un string pt afisare in coloana respectiva
        //
        List<T> rows = items.ToList();

        if(rows.Count==0)
        {
            Console.WriteLine("No data found.");
            return;
        }

        int columnCount = columns.Length;
        int[] columnWidths = new int[columnCount];

        //latime coloanez
        for(int i=0;i<columnCount;i++)
        {
            //row lista de randuri(dto urile aici )
            //columns: array de coloane, fiecare element avand: Header, Selector, Align

            ///max: pe fiecare rand r din rows, ia valoare maxima 
            //selector(r): pt randul curent , aplica functia selector pt coloana i, daca enull intoarce string gol
            //length: lungimea stringului rezultat
            int maxDataWidth = rows.Max(r => (columns[i].Selector(r) ?? string.Empty).Length);
            //latimea maxima a valorilor din coloana i

            //latimea coloanei e max dintre header si latimea maxima a valorilor din coloana
            columnWidths[i]=Math.Max(columns[i].Header.Length, maxDataWidth);
        }

        //header
        for(int i=0;i<columnCount;i++)
        {
            //aliniere header cu acc aliniere ca si coloana  sau pot pune Align.Center
            string header = AlignText(columns[i].Header, columnWidths[i], columns[i].Align);
            Console.Write(header);
            if (i < columnCount - 1)
                Console.Write("  ");
        }
        Console.WriteLine();

        //linie separatoare
        for (int i = 0; i < columnCount; i++)
        {
            Console.Write(new string('-', columnWidths[i]));
            if (i < columnCount - 1)
                Console.Write("  ");
        }
        Console.WriteLine();

        //randuri
        foreach (var row in rows)
        {
            for (int i = 0; i < columnCount; i++)
            {
                //ia coloana i, ia functia ei: s=> s.FullName,o aplica pe randul row curent, pne rez in cell
                string cell = columns[i].Selector(row) ?? string.Empty;
                string aligned = AlignText(cell, columnWidths[i], columns[i].Align);
                Console.Write(aligned);
                if (i < columnCount - 1)
                    Console.Write("  ");
            }
            Console.WriteLine();
        }
    }

    private static string AlignText(string text, int width, Align align)
    {
        text ??= string.Empty;
        if (text.Length >= width)
            return text;

        int padding = width - text.Length;

        return align switch
        {
            Align.Left => text.PadRight(width),
            Align.Right => text.PadLeft(width),
            Align.Center => new string(' ', padding / 2) + text + new string(' ', padding - padding / 2),
            _ => text
        };
    }
}