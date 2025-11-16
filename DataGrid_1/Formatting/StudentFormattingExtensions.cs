using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Formatting;

public static class StudentFormattingExtensions
{
    public static string ShortAge(this Student s)
    {
        var (years, months) = s.GetAge();

        if (years == 0 && months == 0)
            return "0m";
        if (years == 0)
            return $"{months}m";
        if (months == 0)
            return $"{years}y";

        return $"{years}y {months}m";
    }

    public static string LongAge(this Student s)
    {
        var (years, months) = s.GetAge();

        if (years == 0 && months == 0)
            return "0 months old";
        if (years == 0)
            return $"{months} months old";
        if(months == 0)
            return $"{years} years old"; 
        
        return $"{years} years and {months} months old";
    }

    public static string NameWithFather(this Student s)
    {
        if (string.IsNullOrEmpty(s.FatherName))
            return $"{s.FirstName} {s.LastName}";
        return $"{s.FirstName} {s.FatherName[0]}. {s.LastName}";
    }
}
