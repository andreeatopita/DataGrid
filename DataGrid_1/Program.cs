// See https://aka.ms/new-console-template for more information



using DataGrid_1;
using DataGridLib;
using DataGrid_1.Storage;

// incarc datele din fisier
var jsonPath = Path.Combine(AppContext.BaseDirectory, "students.json");
Console.WriteLine($"JSON path: {jsonPath}");

var store = new StudentFileStore(jsonPath);

//incarc din json, sau lista goala daca fisierul nu exista
var students = store.Load();

Console.WriteLine($"Loaded {students.Count} students from JSON.");


var cfg = new GridConfiguration<Student>()
    .RowNumber(true, Alignment.Right) // coloana cu nr randului
    .AddColumn("#", GridDataType.Int, s => s.StudentId)
    .AddColumn("Name", GridDataType.String, s => s.FullName)
    .AddColumn("Father", GridDataType.String, s => s.FatherNameFormat )
    .AddColumn("Date of Birth", GridDataType.DateTime, s => s.DateOfBirthFormat)
    .AddColumn("Age", GridDataType.String, s => s.StudentAge)
    .AddColumn("Active", GridDataType.Bool, s => s.ActiveStatus)
    .AddColumn("Last Active", GridDataType.DateTime, s => s.LastActiveAtFormat)
    .AddColumn("Account Balance", GridDataType.Decimal, s => s.AccountBalanceFormat);

Console.WriteLine("--Students--");
GridDataSource<Student> dataSourceA = new GridDataSource<Student>(students);
var gridA= new DataGrid<Student>(cfg, dataSourceA);
gridA.Display();
Console.WriteLine();


Console.WriteLine("--Students_Partial--");
gridA.Display("#", "Name", "Active");
Console.WriteLine();

Console.WriteLine("--Students_Active_Only_LastName--");
cfg.Where(s => s.IsActive)
    .OrderBy(s => s.LastName);
gridA.Display();
Console.WriteLine();



cfg.ResetQuery();
Console.WriteLine("--Students_Ordered by LastName--");
cfg.OrderBy(s => s.LastName);
gridA.Display();
Console.WriteLine();

//subset de randuri, skip : sare peste primele n randuri
cfg.ResetQuery(); //resetez filtrarea, ordonarea, skip, take
cfg.Skip(2);
Console.WriteLine("--Students_Skip 2--");
gridA.Display();
Console.WriteLine();


//take : ia primele n randuri
cfg.ResetQuery(); //resetez filtrarea, ordonarea, skip, take
cfg.Take(5);
Console.WriteLine("--Students_A_Skip 2 Take 5--");
gridA.Display();
Console.WriteLine();

cfg.ResetQuery(); //resetez filtrarea, ordonarea, skip, take

//adaug un student:

//nu dublez id ul
int nextId= students.Any() ? students.Max(s => s.StudentId) + 1 : 1;

var newStudent = new Student(
    id: nextId,
    firstName: "Eva",
    lastName: "Doe",
    dateOfBirth: new DateOnly(2002, 3, 14),
    isActive: true,
    fatherName: "John",
    lastActiveAt: DateTime.Now,
    accountBalance: 1500m
);

/*
students.Add(newStudent);

if (store.Save(students))
{
    Console.WriteLine("New student saved to JSON.");
}
else
{
    Console.WriteLine("Failed to save new student to JSON.");
}

Console.WriteLine();
Console.WriteLine("--Students_After_Adding_New_Student--");
gridA.Display();
Console.WriteLine();
*/

//update student

//am private set pe proprietati si nu pot modifica direct un obiect existent, deci adaug metode

var studentToUpdate = students.FirstOrDefault(s => s.StudentId == 7);
if (studentToUpdate != null)
{

    studentToUpdate.UpdateAccountBalance(2300m);
    studentToUpdate.UpdateLastActiveAt(DateTime.Now);
    studentToUpdate.UpdateFatherName("Daniel");
    studentToUpdate.Deactivate();

    if (store.Save(students))
    {
        Console.WriteLine("Student updated and saved to JSON.");
    }
    else
    {
        Console.WriteLine("Failed to save updated student to JSON.");
    }
}
else
{
    Console.WriteLine("Student with ID 2 not found for update.");
}

Console.WriteLine();
Console.WriteLine("--Students_After_Updating_Student--");
gridA.Display();