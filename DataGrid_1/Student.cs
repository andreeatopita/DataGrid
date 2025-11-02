using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1;

public class Student
{
    public int StudentId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    //optional numele tatalui, poate fi null
    public string? FatherName { get; private set; } 

    public DateOnly DateOfBirth { get; private set; }
    public DateTime? LastActiveAt { get; private set; }
    public decimal AccountBalance { get; private set; }
    public bool IsActive { get; private set; }


    //proprietati pt afisare in format dorit
    public string ActiveStatus => IsActive ? "Yes" : "No";
    public string DateOfBirthFormat => DateOfBirth==default ? "N/A" : DateOfBirth.ToString("yyyy-MM-dd");
    public string FullName => $"{FirstName} {LastName}";
    public string AccountBalanceFormat => $"{AccountBalance:C}"; //simbol valuta 
    public string LastActiveAtFormat => LastActiveAt.HasValue ? LastActiveAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : "N/A";
    public string FatherNameFormat => FatherName ?? "N/A";


    //de reparat studentage
    public string StudentAge
    {
        get
        { 
            if(DateOfBirth==default)
            {
                return "N/A";
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            int year = today.Year - DateOfBirth.Year;
            int month = today.Month - DateOfBirth.Month;

            if(today.Day< DateOfBirth.Day)
            {
                month--;
            }

            if (month < 0)
            {
                year--;
                month += 12;
            }

            return $"{year}y {month}m ";
        }
    }
    public Student(int id,string firstName, string lastName, DateOnly dateOfBirth, bool isActive, string? fatherName = null, DateTime? lastActiveAt = null, decimal accountBalance = 0)
    {
        StudentId = id;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        IsActive = isActive;

        FatherName = fatherName;
        LastActiveAt = lastActiveAt;
        AccountBalance = accountBalance;
    }

    public Student() :this(0,string.Empty,string.Empty,default,false) {}


    //daca vreau sa modific un student, folosesc o metoda 
    public void UpdateAccountBalance(decimal newBalance)
    {
        AccountBalance = newBalance;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void UpdateLastActiveAt(DateTime lastActive)
    {
        LastActiveAt = lastActive;
    }

    public void UpdateFatherName(string? fatherName)
    {
        FatherName = fatherName;
    }



}

