using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGrid_1.Dto;

//DTO - Data Transfer Object
public class StudentDto
{
    public int StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty; 
    public string LastName { get; set; } = string.Empty;
    public string? FatherName { get; set; }   
    public DateTime DateOfBirth { get; set; }
    public DateTime? LastActiveAt { get; set; }  //primesc null din json 
    public decimal AccountBalance { get; set; }
    public bool IsActive { get; set; }
}
