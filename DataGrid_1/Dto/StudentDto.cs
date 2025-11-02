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
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FatherName { get; set; }        
    public DateOnly DateOfBirth { get; set; }
    public DateTime? LastActiveAt { get; set; }    
    public decimal AccountBalance { get; set; }
    public bool IsActive { get; set; }
}
