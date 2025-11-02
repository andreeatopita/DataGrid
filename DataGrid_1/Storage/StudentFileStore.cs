using DataGrid_1.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataGrid_1.Storage;

public class StudentFileStore
{
    private readonly string _path;
    private readonly JsonSerializerOptions _json = new() { WriteIndented = true };

    public StudentFileStore(string path)
    {
        _path = path;
    }

    //citeste din json si ret lista de studenti 
    public List<Student> Load()
    {
        if (!File.Exists(_path))
        {
            Console.Error.WriteLine($"File not found: {_path}. Starting empty.");
            return new List<Student>();
        }

        try
        {
            var json = File.ReadAllText(_path);
            var dtos = JsonSerializer.Deserialize<List<StudentDto>>(json, _json) ?? new List<StudentDto>();
            return dtos.Select(FromDto).ToList();
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"Invalid JSON '{_path}': {ex.Message}");
            return new List<Student>(); // continui curat
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.Error.WriteLine($"Access denied, cannot readL: '{_path}': {ex.Message}");
            return new List<Student>();
        }
    }

    // scrie in JSON
    public bool Save(List<Student> students)
    {
        try
        {
            var dtos = students.Select(ToDto).ToList();
            var json = JsonSerializer.Serialize(dtos, _json);
            File.WriteAllText(_path, json);

            return true;
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine($"Write error '{_path}': {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.Error.WriteLine($"Access denied '{_path}': {ex.Message}");
        }

        return false;
    }


    //from dto to student
    private static Student FromDto(StudentDto d) => new Student(
        id: d.StudentId,
        firstName: d.FirstName ?? string.Empty,
        lastName: d.LastName ?? string.Empty,
        dateOfBirth: d.DateOfBirth,
        isActive: d.IsActive,
        fatherName: d.FatherName,
        lastActiveAt: d.LastActiveAt,
        accountBalance: d.AccountBalance
    );

    private static StudentDto ToDto(Student s) => new StudentDto
    {
        StudentId = s.StudentId,
        FirstName = s.FirstName,
        LastName = s.LastName,
        FatherName = s.FatherName,
        DateOfBirth = s.DateOfBirth,
        LastActiveAt = s.LastActiveAt,
        AccountBalance = s.AccountBalance,
        IsActive = s.IsActive
    };
}