using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AccountLib;
namespace DataGrid_1;

public class Student
{
    public int StudentId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    //optional numele tatalui, poate fi null
    public string? FatherName { get; private set; } 
    //dateonly?
    public DateTime DateOfBirth { get; private set; }
    public DateTime LastActiveAt { get; private set; }
    //sold cont - dinamic, preia din account

    public decimal AccountBalance => Account.Balance;
    public bool IsActive { get; private set; }

    //tuple pt varsta
    public (int Years, int Months) GetAge()
    {
        if (DateOfBirth == default) 
            return (0, 0);

        DateTime today = DateTime.Today;
        int years = today.Year - DateOfBirth.Year;
        int months = today.Month - DateOfBirth.Month;

        if (today.Day < DateOfBirth.Day)
            months--;

        if (months < 0)
        {
            years--;
            months += 12;
        }
        return (years, months);
    }


    private Account Account { get; } = new Account();
    public string FullName => $"{FirstName} {LastName}";

    public Student(int id,string firstName, string lastName, DateTime dateOfBirth, bool isActive, string? fatherName = null, DateTime? lastActiveAt = null, decimal initialBalance = 0)
    {
        StudentId = id;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        IsActive = isActive;

        FatherName = fatherName;
        //daca lastactiveat e default, pun data curenta, altfel pun ce am primit
        //LastActiveAt = lastActiveAt== default ? DateTime.Now : lastActiveAt;

        LastActiveAt = LastActiveAt = lastActiveAt ?? DateTime.Now;


        if (initialBalance>0)
            Account.Receive(initialBalance, DateTime.Now); // tranzactie
    }

    public Student() :this(0,string.Empty,string.Empty,default,false) {}

    //daca vreau sa modific un student, folosesc o metoda 

    public void Activate()
    {
        //daca e fals, il activez
        if (IsActive == false)
        {
            IsActive = true;
            LastActiveAt= DateTime.Now;
        }
    }

    public void Deactivate()
    {
        //daca e activat, il dezactivez, lastactiveat ramane neschimbat
        if (IsActive == true)
            IsActive = false;
    }

    public void UpdateLastActiveAt(DateTime lastActive)
    {
        //daca e default, pun data curenta, altfel pun ce am primit
        LastActiveAt = lastActive == default ? DateTime.Now : lastActive;
    }

    public void UpdateFatherName(string? fatherName)
    {
        FatherName = fatherName;
    }


    //adaugare bani:
    public void ReceiveMoney(decimal amount, DateTime? date=null) => Account.Receive(amount, date);
    public void SpendMoney(decimal amount, DateTime? date=null) => Account.Spend(amount, date);


    //metode legate de account:
    //returnez toate tranzactiile
    public IEnumerable<TransactionInfo> AllTransactions() => Account.GetTransactions();

    public IEnumerable<TransactionInfo> ReceivedTransactions() => Account.GetReceived();

    public IEnumerable<TransactionInfo> SpentTransactions() => Account.GetSpent();

    public IEnumerable<TransactionInfo> RecentReceivedAbove(decimal minAmount, int days) => Account.GetRecentReceivedAbove(minAmount, days);

    //metoda bool pentru a verifica daca exista tranzactii de tip received 
    public bool HasRecentReceivedAbove(decimal minAmount, int days) =>
        RecentReceivedAbove(minAmount, days).Any();

    //returneaza suma maxima a tranzactiilor de tip received in ultimele days zile, peste minAmount
    public decimal MaxRecentReceivedAbove(decimal minAmount, int days) =>
        RecentReceivedAbove(minAmount, days)
            .Select(t => t.Amount)
            .DefaultIfEmpty(0) //daca nu exista tranzactii, returneaza 0
            .Max();

    //returneaza data tranzactiei cu suma maxima care indeplineste conditiile
    public DateTime MaxRecentReceivedDate(decimal minAmount, int days)
    {
        TransactionInfo? tx = RecentReceivedAbove(minAmount, days)
            .OrderByDescending(t => t.Amount)
            .ThenByDescending(t => t.Date)
            .FirstOrDefault();
        return tx?.Date ?? default;
    }

}

