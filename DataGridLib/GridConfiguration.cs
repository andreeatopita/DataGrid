using DataGridLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridLib;

//descriere coloane, 
public class GridConfiguration<T> : IGridConfiguration<T>
{
    //lista de coloane
    public List<IColumn<T>> Columns { get; }
    public bool ShowRowNumber { get; private set; }
    public Alignment RowNumberAligment { get; private set; }

    //transformari peste IEnumerable<T>, filtre ordonari paginare , repr colectia de T
    private Func<IEnumerable<T>, IEnumerable<T>> queryModifier = items => items;


    //filtrare cu where si predicat
    //predicate = functie care returneaza bool, decide daca un element T trebuie inclus in rezultat
    // pastreaza doar elementele pentru care predicatul e true
    public GridConfiguration<T> Where(Func<T, bool> predicate)
    {
        //prev - pentru a pastra vechea functie, in caz ca vreau sa o folosesc
        var prev = queryModifier;
        queryModifier = items => prev(items).Where(predicate);
        return this;
    }
    //ordonare dupa o cheie 
    public GridConfiguration<T> OrderBy<TKey>(Func<T, TKey> keySelector, bool desc = false)
    {
        // pastrez vechea functie
        var prev = queryModifier;
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

    //pentru a putea afisa subseturi:
    //sare peste primele n elemente ( pt paginare)
    public GridConfiguration<T> Skip(int n)
    {
        // pastrez vechea functie
        var prev = queryModifier;
        queryModifier = items => prev(items).Skip(n);
        return this;
    }

    //ia primele n elemente
    public GridConfiguration<T> Take(int n)
    {
        //prev - pastrez vechea functie
        var prev = queryModifier;
        queryModifier = items => prev(items).Take(n);
        return this;
    }


    //aplica transformarile definite in queryModifier peste sursa data
    internal IEnumerable<T> Apply(IEnumerable<T> source) => queryModifier(source);
    //ruleaza transformarile definite in queryModifier peste sursa data
    //internal - doar in proiectul curent, bine pentru incapsulare , nu expun in afara librariei
    //console app foloseste data grid nu apply direct
    //applu - executa comenzile query, returneaza rezultatul final


    public GridConfiguration()
    {
        Columns = new List<IColumn<T>>();
        ShowRowNumber = false;
        RowNumberAligment = Alignment.Right;
    }

    //rownumber pentru afisarea numerelor de rand
    public GridConfiguration<T> RowNumber(bool show = true, Alignment alignment = Alignment.Right)
    {
        ShowRowNumber = show;
        RowNumberAligment = alignment;
        return this;
    }

    //adaugare coloane
    public GridConfiguration<T> AddColumn<TProp>(string header, GridDataType dataType, Func<T, TProp> valueGetter, Alignment? alignment = null)
    {
        Columns.Add(new Column<T, TProp>(header, dataType, valueGetter, alignment));
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
    //pentru resetarea transformarilor aplicate!!!!! orderby skip take where
    public GridConfiguration<T> ResetQuery()
    {
        queryModifier = items => items; // identitate
        return this;
    }
}
