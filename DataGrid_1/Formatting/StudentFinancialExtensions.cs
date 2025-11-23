using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Formatting;

public static class StudentFinancialExtensions
{
    public static void SpendMoneySafe(this Student s, decimal amount)
    {
        try
        {
            s.SpendMoney(amount);
            Console.WriteLine($"Spent {amount}. New balance: {s.AccountBalance}.\n");
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"Cannot spend {amount}. Current balance: {s.AccountBalance}.\n");
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("Amount must be greater than zero.\n");
        }
    }

    public static void ReceiveSafe(this Student s, decimal amount)
    {
        try
        {
            s.ReceiveMoney(amount);
            Console.WriteLine($"Received {amount}. New balance: {s.AccountBalance}.\n");
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("Amount must be greater than zero.\n");
        }
    }
}
