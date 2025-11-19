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
            var result = new List<Student>();
            foreach (var d in dtos)
            {
                //construiesc student fara balance initial (se reconstruieste din tranzactii)
                var s = new Student(
                    id: d.StudentId,
                    firstName: d.FirstName,
                    lastName: d.LastName,
                    dateOfBirth: d.DateOfBirth,
                    isActive: d.IsActive,
                    fatherName: d.FatherName,
                    lastActiveAt: d.LastActiveAt ?? default,
                    initialBalance: 0m);

                // daca lista de tranzactii e null, o initializez cu lista goala
                d.Transactions ??= new List<TransactionDto>();

                //reconstruiesc istoricul tranzactiilor in cont, ordonat dupa data
                foreach (var t in d.Transactions.OrderBy(t => t.Date))
                {
                    if (string.Equals(t.Type, "Received", StringComparison.OrdinalIgnoreCase))
                        s.ReceiveMoney(t.Amount, t.Date);

                    else if (string.Equals(t.Type, "Spent", StringComparison.OrdinalIgnoreCase))
                        s.SpendMoney(t.Amount, t.Date);
                }
                result.Add(s);
            }

            return result;
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
    //from student to dto, salvare in json
    private static StudentDto ToDto(Student s) => new StudentDto
    {
        StudentId = s.StudentId,
        FirstName = s.FirstName,
        LastName = s.LastName,
        FatherName = s.FatherName,
        DateOfBirth = s.DateOfBirth,
        LastActiveAt = s.LastActiveAt,
        IsActive = s.IsActive,
        Transactions = s.AllTransactions()
        .Select(tr => new TransactionDto
        {
            Amount = tr.Amount,
            Date = tr.Date,
            Type = tr.Type.ToString()
        }).ToList(),
    };
}