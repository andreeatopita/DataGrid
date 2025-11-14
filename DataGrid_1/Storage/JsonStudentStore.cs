using DataGrid_1.Dto;
using System.Text.Json;


namespace DataGrid_1.Storage;

//async,await, task 
//replace : json


public class JsonStudentStore : IJsonStudentStore
{
    //calea catre fisierul json
    private readonly string _path;
    //optimizari pentru json, formatat json usor de citit, object initializer
    private readonly JsonSerializerOptions _json = new() { WriteIndented = true };

    public JsonStudentStore(string path)
    {
        _path = path;
    }

    //citeste din json si ret lista de studenti 
    public List<Student> Load()
    {
        if (!File.Exists(_path))
        {
            //lista goala si mesaj de eroare
            Console.WriteLine($"File not found: {_path}. Starting empty.");
            return new List<Student>();
        }

        //metoda de citire
        try
        {
            //citesc tot continutul fisierului, il transf in lista de studentdto,dupa care in lista de studenti
            string json = File.ReadAllText(_path);
 
            //deserializare: din json in obiecte 
            List<StudentDto> dtos = JsonSerializer.Deserialize<List<StudentDto>>(json, _json) ?? new List<StudentDto>();
            return dtos.Select(FromDto).ToList();
        }
        catch (JsonException ex)
        {
            //json exception: eroare la parsarea json-ului, caractere in plus, format incorect
            Console.WriteLine($"Invalid JSON {_path}: {ex.Message}");
            return new List<Student>(); 
        }
        catch (UnauthorizedAccessException ex)
        {
            //eroare de acces la fisier, permisiuni insuficiente,fisier protejat 
            Console.WriteLine($"Access denied, cannot read: {_path}: {ex.Message}");
            return new List<Student>();
        }
        catch(IOException ex)
        {
            //eroare generica de intrare/iesire la citirea fisierului
            Console.WriteLine($"Read error {_path}: {ex.Message}");
            return new List<Student>();
        }
    }

    //salvare lista de studenti in fisier json, true daca reusit, false altfel
    public bool Save(List<Student> students)
    {
        try
        {
            List<StudentDto> dtos = students.Select(ToDto).ToList();
            //serializare: din obiecte in json, convertesc lista de dto in json
            string json = JsonSerializer.Serialize(dtos, _json);

            //daca fisierul nu exista, il creeaza, daca exista, il suprascrie
            File.WriteAllText(_path, json);

            return true;
        }
        catch (IOException ex)
        {
            //erorile de intrare/iesire la scrierea fisierului
            Console.WriteLine($"Write error {_path}: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            //eroare de acces la fisier, permisiuni insuficiente,fisier protejat
            Console.WriteLine($"Access denied {_path}: {ex.Message}");
        }

        return false;
    }


    //mapare dto in student pentru incarcare din json
    private static Student FromDto(StudentDto d) => new Student(
        //student nou cu proprietatile din dto
        id: d.StudentId, //id din json in student
        firstName: d.FirstName,
        lastName: d.LastName,
        dateOfBirth: d.DateOfBirth,
        isActive: d.IsActive,
        fatherName: d.FatherName,
        //daca e null -> pun default -> constructorul din student pune data curenta
        lastActiveAt: d.LastActiveAt ?? default, 
        accountBalance: d.AccountBalance
    );

    //from student to dto, salvare in json
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