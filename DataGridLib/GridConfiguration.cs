using DataGridLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib;

//tipul de entitate pe care il afiseaza grila: student,....
public class GridConfiguration<T> : IGridConfiguration<T>
{
    public List<IColumn<T>> Columns { get; }
    public bool ShowRowNumber { get; private set; }
    public Alignment RowNumberAligment { get; private set; }

    private Func<IEnumerable<T>, IEnumerable<T>> queryModifier = items => items;
    //filtrari, ordonari. functie de la secv la secv , fiecare metoda: where orderby skip modifica functia anterioara, lant
    //iau o secventa de T, inotrc o secventa de T


    //filtrare cu where si predicat
    //pastreaza doar elementele pentru care predicatul e true
    public GridConfiguration<T> Where(Func<T,bool> predicate)
    {
        //prev - pentru a pastra vechea functie, o incapsulez intr o variabila
        //apoi creez o noua functie care aplica vechea functie si apoi filtreaza cu where
        Func<IEnumerable<T>, IEnumerable<T>> prev = queryModifier;
        queryModifier = items => prev(items).Where(predicate);
        return this;
    }

    //ordonare dupa o cheie aleasa, tkey - tipul cheii dupa care ordonez, adica string, int, datetime
    public GridConfiguration<T> OrderBy<TKey>(Func<T, TKey> keySelector, bool desc = false)
    {
        // pastrez vechea functie
        Func<IEnumerable<T>, IEnumerable<T>> prev = queryModifier;
        //daca desc e true, ordonez descrescator, altfel crescator
        if (desc)
        {
            queryModifier = items => prev(items).OrderByDescending(keySelector);
        }
        else
        {
            queryModifier = items => prev(items).OrderBy(keySelector);
        }
        return this;
    }


    //sare peste primele n elemente ( pt paginare)
    public GridConfiguration<T> Skip(int n)
    {
        // pastrez vechea functie
        Func<IEnumerable<T>, IEnumerable<T>> prev = queryModifier;
        queryModifier = items => prev(items).Skip(n);
        return this;
    }

    //ia primele n elemente
    public GridConfiguration<T> Take(int n)
    {
        //prev - pastrez vechea functie
        Func<IEnumerable<T>, IEnumerable<T>> prev = queryModifier;
        queryModifier = items => prev(items).Take(n);
        return this;
    }


    //aplica transformarile definite in queryModifier peste sursa data, aplica lantul peste datasource
    internal IEnumerable<T> Apply(IEnumerable<T> source) => queryModifier(source);
    //ruleaza transformarile definite in queryModifier peste sursa data


    public GridConfiguration()
    {
        Columns = new List<IColumn<T>>();
        ShowRowNumber = false;
        RowNumberAligment = Alignment.Left;
    }

    //rownumber pentru afisarea numerelor de rand
    public GridConfiguration<T> RowNumber(bool show = true, Alignment alignment = Alignment.Left)
    {
        ShowRowNumber = show;
        RowNumberAligment = alignment;
        return this;
    }

    //adaugare coloane care stie cum sa extraga o proprietate din T, si ce tip de date are, pentru aliniere implicita
    public GridConfiguration<T> AddColumn<TProp>(string header, Func<T, TProp> valueGetter, Alignment? alignment = null, Func<TProp, string>? cellFormatter = null)
    {
        Columns.Add(new Column<T, TProp>(header, valueGetter, alignment, cellFormatter));
        return this;
    }

    public void Validate()
    {
        if (Columns.Any(c => string.IsNullOrEmpty(c.Header)))
        {
            throw new InvalidOperationException("All columns must have a header.");
        }

        if (Columns.Count == 0)
        {
            throw new InvalidOperationException("At least one column must be defined.");
        }

    }
    //pentru resetarea transformarilor aplicate
    public GridConfiguration<T> ResetQuery()
    {
        //queryModifier - resetez la functia initiala care returneaza elementele nemodificate
        queryModifier = items => items; 
        return this;
    }
}
