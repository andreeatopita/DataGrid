using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataGrid_1.Formatting.Interfaces;

namespace DataGrid_1.Formatting;

public class CurrencyFormatter : ICurrencyFormatter
{
    public CultureInfo Culture { get; }
    public string? Prefix { get; }
    public string? Suffix { get; }

    //permit null si setez implicit cand nu trec acel argument
    public CurrencyFormatter(CultureInfo? culture=null,string? prefix=null, string? suffix=null)
    {
        //daca primesc cultura null, folosesc cultura curenta, altfel folosesc cultura primita
        Culture = culture ?? CultureInfo.CurrentCulture;
        Prefix = prefix; 
        Suffix = suffix;
    }

    public string FormatCurrency(decimal amount)
    {

        //G - fara separatori, fara zecimale daca e intreg, cu zecimale daca are
        //Culture - in fuctie de cultura, folosesc separatori specifici culturii , sau . pt zecimale
        string numberStr =amount.ToString("G",Culture);
       
        //daca am prefix, il pun in fata nr fara spatiu
        if(!string.IsNullOrEmpty(Prefix))
        {
            return $"{Prefix}{numberStr}";
        }

        //daca am sufix adaug spatiu si sufix
        if (!string.IsNullOrEmpty(Suffix))
        {
            return $"{numberStr} {Suffix}";
        }

        return numberStr;
    }

    //static factory method: primeste parametru cultureinfo si creeaza currencyformatter
    public static CurrencyFormatter CreateFromCulture(CultureInfo culture)
    {
        //pe baza culturii, aleg formatarea potrivita
        return culture.Name switch
        {
            "en-US" => new CurrencyFormatter(culture,prefix: "$"),
            "ro-RO" => new CurrencyFormatter(culture,suffix: "LEI"),
            "fr-FR" => new CurrencyFormatter(culture,suffix: "€"),
            _ => new CurrencyFormatter(culture,prefix: culture.NumberFormat.CurrencySymbol),
        };
        //culture.NumberFormat - number format info pt cultura respectiva
        //CurrencySymbol - simbolul monedei pt cultura respectiva, folosesc simbolul valutei din cultura resp ca prefix
    }
}
