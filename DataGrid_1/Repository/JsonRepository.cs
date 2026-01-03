using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AccountLib;
using DataGridLib.Contracts;
using DataGrid_1.Repository.Dtos.Json;

namespace DataGrid_1.Repository;

public class JsonRepository : IRepository<Student>
{
    private readonly string filePath;
    
    private readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    //doar o operatie poate citi/scrie in fisier la un moment dat
    //1- cate sunt libere la inceput, 1- cate pot fi max
    //static - shared intre toate instantele clasei
    private static readonly SemaphoreSlim semaphore = new(1, 1);

    public JsonRepository(string path)
    {
        filePath = path;
    }


    //incarcare studenti din fisier
    //task: operatie asincrona care returneaza o lista de studenti
    //await: astept finalizarea operatiei asincrone
    //async: metoda e asincrona, load incarca studentii din fisier
    public async Task<IReadOnlyList<Student>> LoadAsync()
    {
        //astept sa pot citi fisierul, pt ca alt proces poate scrie in el
        //pui la coada fara a bloca thread ul curent
        await semaphore.WaitAsync();

        try
        {
            if(!File.Exists(filePath))
            {
                //daca nu exista intorc lista goala
                return Array.Empty<Student>();
            }

            //stream pt citire 
            //la final apeleaza disposeasync() , nu blocheaza thread ul 
            await using FileStream stream = File.OpenRead(filePath);

            //deserializare din json in lista de studentdto 
            //daca e null, intorc lista vida
            List<StudentDto> dtos = await JsonSerializer.DeserializeAsync<List<StudentDto>>(stream, jsonOptions) ?? new List<StudentDto>();

            //transform fiecare dto in entity student
            //fac lista de student din lista de dto
            List<Student> result = new List<Student>(dtos.Count);

            //adaug in lista finala studentii transformati din dto
            foreach (StudentDto dto in dtos)
            {
                //reconstruire istoric tranzactii 
                result.Add(MapToEntity(dto));
            }

            //returnez lista de studenti 
            return result;

        }
        catch (JsonException ex)
        {
            //aici de ce error?
            Console.WriteLine($"Invalid JSON: {filePath}: {ex.Message}");
            return Array.Empty<Student>();
        }
        catch(IOException ex)
        {
            Console.WriteLine($"Read Error: {filePath}: {ex.Message}");
            return Array.Empty<Student>();
        }
        finally
        {
            //eliberez semaphore-ul ca sa poata alt proces citi/scrie
            semaphore.Release();
        }
    }
    //load: deserializeaza din json -> dto -> entity, recontruieste tranz in ord cronologica 

    //salvare studenti in fisier
    public async Task SaveAsync(IEnumerable<Student> items)
    {
        if(items == null)
            throw new ArgumentNullException(nameof(items));

        //entitati -> dto 
        List<StudentDto> dtoList = items.Select(MapToDto).ToList();

        //semaphore pt a nu scrie in fisier cand alt proces il citeste/scrie
        await semaphore.WaitAsync();

        try
        {
            string? dir = Path.GetDirectoryName(filePath);

            //creez directorul daca nu exista
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            //deschide filestream cu file create ( creeaza sau suprascrie daca exista)
            //file access write si fileshare none 
            //disposeasync la final
            await using FileStream stream = File.Create(filePath);

            //serializare dto list in json
            await JsonSerializer.SerializeAsync(stream, dtoList, jsonOptions);

            //flush pt a ma asigura ca totul e scris in fisier
            await stream.FlushAsync();

        }
        catch (IOException ex)
        {
            Console.WriteLine($"Write Error: {filePath}: {ex.Message}");
        }
        finally
        {
            //eliberez semaphore-ul ca sa poata alt proces citi/scrie
            semaphore.Release();
        }
    }
    //save : entity -> dto -> serialize json 

    //serializare pt json
    //iau student( account privat ) si in transform in dto ( cu lista de tranzactii publice )
    private static StudentDto MapToDto(Student s) =>
        new StudentDto(
            s.StudentId,
            s.FirstName,
            s.LastName,
            s.FatherName,
            s.DateOfBirth,
            s.LastActiveAt,
            s.IsActive,
            s.AllTransactions()
                .Select(tr => new TransactionDto(tr.Amount, tr.Date, tr.Type))
                .ToList()
        );
    //list de transaction dto 


    //map to entity
    //deserializare din json
    private static Student MapToEntity(StudentDto dto)
    {
        Student s=new Student(dto.StudentId, dto.FirstName, dto.LastName, dto.DateOfBirth,dto.IsActive, dto.FatherName,dto.LastActiveAt, 0);

        //daca nu are tranzactii, initializez cu lista vida
        //lista de tranzactii in json
        //dto.Transactions e lista din studentdto
        var history = (dto.Transactions ?? new List<TransactionDto>())
                  .Select(t => new TransactionInfo(t.Amount, t.Date, t.Type));

        s.LoadHistory(history);

        return s;
    }
}

