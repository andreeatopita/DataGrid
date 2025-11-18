// See https://aka.ms/new-console-template for more information



using DataGrid_1;
using DataGrid_1.Formatting;
using DataGrid_1.Grids;
using DataGrid_1.Storage;
using DataGridLib;
using System.Globalization;
using System.Text;
using DataGrid_1.Export;
using DataGridLib.Export;
using DataGridLib.Export.Interfaces;

Console.OutputEncoding = Encoding.UTF8;


// incarc datele din fisier 
string jsonPath = Path.Combine(AppContext.BaseDirectory, "students.json");
Console.WriteLine($"JSON path: {jsonPath}");

//creare store care gest citirea/scrierea json la calea respectiva 
JsonStudentStore store = new JsonStudentStore(jsonPath);

//citeste fisierul json si returneaza lista de studenti, daca nu ex fis, returneaza lista goala
List<Student> students = store.Load();

Console.WriteLine($"Loaded {students.Count} students from JSON.");

var cfg = new GridConfiguration<Student>()
    .RowNumber(true) // coloana cu nr randului
    .AddColumn("Id", s => s.StudentId)
    .AddColumn("Name", s => s.FullName)
    .AddColumn("Father", s => s.FatherName)
    .AddColumn("Date of Birth", s => s.DateOfBirth, cellFormatter: d => d.ToString("dd.MM.yyyy"))
    .AddColumn("Age", s => s.ShortAge())
    .AddColumn("Active", s => s.IsActive)
    //.AddColumn("Active", s => s.IsActive, cellFormatter: b=> b ? "Da" : "Nu")
    .AddColumn("Last Active", s => s.LastActiveAt)
    //.AddColumn("Last Active", s => s.LastActiveAt, Alignment.Center, dt => dt.ToString("yyyy-MM-dd"))
    .AddColumn("Account Balance", s => s.AccountBalance);

Console.WriteLine("--Students--");

GridDataSource<Student> dataSourceA = new GridDataSource<Student>(students);
var gridA= new DataGrid<Student>(cfg, dataSourceA);

gridA.Display();
Console.WriteLine();


Console.WriteLine("--Students_Partial--");
gridA.Display("Id", "Name", "Active");
Console.WriteLine();

Console.WriteLine("--Students_Active_Only_LastName--");
cfg.Where(s => s.IsActive)
    .OrderBy(s => s.LastName);
gridA.Display();
Console.WriteLine();



cfg.ResetQuery();
Console.WriteLine("--Students_Ordered_by_LastName--");
cfg.OrderBy(s => s.LastName);
gridA.Display();
Console.WriteLine();

//subset de randuri, skip : sare peste primele n randuri
cfg.ResetQuery(); //resetez filtrarea, ordonarea, skip, take
cfg.Skip(2);
Console.WriteLine("--Students_Skip_2--");
gridA.Display();
Console.WriteLine();


//take: ia primele n randuri
cfg.ResetQuery(); //resetez filtrarea, ordonarea, skip, take
cfg.Take(5);
Console.WriteLine("--Students_A_Take_5--");
gridA.Display();
Console.WriteLine();

cfg.ResetQuery(); //resetez filtrarea, ordonarea, skip, take

//adaug un student:
//nu dublez id ul
int nextId;
//ia cel mai mare id si adauga 1 pentru urmatorul id
if (students.Any())
    nextId = students.Max(s => s.StudentId) + 1;
else
    nextId = 1;


var newStudent = new Student(
        id: nextId,
        firstName: "Eva",
        lastName: "Doe",
        dateOfBirth: new DateTime(2002, 3, 14),
        isActive: true,
        fatherName: "John",
        lastActiveAt: DateTime.Now,
        initialBalance: 1500m);
//initial pentru tranzactii

//aici pentru adaugarea unui student 
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

/*
var studentToUpdate = students.FirstOrDefault(s => s.StudentId == 8);
if (studentToUpdate != null)
{
    studentToUpdate.ReceiveMoney(500,new DateTime(2024,1,2);
    studentToUpdate.UpdateFatherName("Noad");
    studentToUpdate.Deactivate();
    //studentToUpdate.SpendMoneySafe(1000);

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
    Console.WriteLine("Student not found for update.");
}
*/

Console.WriteLine();
Console.WriteLine("--Students_After_Updating_Student--");
gridA.Display();


//EX 2 studenti inactivi
Console.WriteLine("\n--Students_Inactive--\n");
GridConfiguration<Student> inactiveCfg = new InactiveStudentsGrid().Build();
DataGrid<Student> inactiveGrid = new DataGrid<Student>(inactiveCfg, dataSourceA);
inactiveGrid.Display();

//EX 3 studenti cu account balance >= 200 
Console.WriteLine("\n--Students_Balance_Above_200_Default--\n");
GridConfiguration<Student> balanceCfg = new StudentsByBalanceGrid().Build();
DataGrid<Student> balanceGrid = new DataGrid<Student>(balanceCfg, dataSourceA);
balanceGrid.Display();

//EX 3 CULTURA
Console.WriteLine("\n--Students_Balance_Above_200_RO_Culture--\n");
GridConfiguration<Student> balanceCfgRO = new StudentsByBalanceGrid(new CultureInfo("ro-RO")).Build();
DataGrid<Student> balanceGridRO = new DataGrid<Student>(balanceCfgRO, dataSourceA);
balanceGridRO.Display();

//EX 3 sufix
Console.WriteLine("\n--Students_Balance_Above_200_Custom_Suffix--\n");
GridConfiguration<Student> balanceCfgCustom = new StudentsByBalanceGrid(new CultureInfo("fr-FR"), suffix: "Eur").Build();
DataGrid<Student> balanceGridCustom = new DataGrid<Student>(balanceCfgCustom, dataSourceA);
balanceGridCustom.Display();

//ex 3 cu ja
Console.WriteLine("\n--Students_Balance_Above_200_Custom_Prefix--\n");
GridConfiguration<Student> balanceCfgJP = new StudentsByBalanceGrid(new CultureInfo("ja-JP")).Build();
DataGrid<Student> balanceGridJP = new DataGrid<Student>(balanceCfgJP, dataSourceA);
balanceGridJP.Display();


//EX 5
Console.WriteLine("\n--Students_Recent_High_Value_Transactions--\n");
GridConfiguration<Student> recentHighCfg = new RecentHighValueTransactionsGrid(culture: new CultureInfo("ro-RO")).Build();
DataGrid<Student> recentHighGrid = new DataGrid<Student>(recentHighCfg, dataSourceA);
recentHighGrid.Display();


//EX 6 : paginare 

//pornesc paginarea :
cfg.EnablePagination(2);

Console.WriteLine("\n-- Page 1 (after EnablePagination) --\n");
gridA.Display();

Console.WriteLine("\n-- NextPage --\n");
gridA.Next();
gridA.Display();

Console.WriteLine("\n-- GotToPage5 --\n");
gridA.GoToPage(3);
gridA.Display();


Console.WriteLine("\n-- Previous Page --\n");
gridA.Previous();
gridA.Display();

Console.WriteLine("\n-- LastPage --\n");
gridA.Last();
gridA.Display();


Console.WriteLine("\n-- FirstPage --\n");
gridA.First();
gridA.Display();

Console.WriteLine("\n-- ChangePageSize --\n");
gridA.ChangePageSize(5);
gridA.Display();


//export in format

//EX 4 export datagrid to csv or xml
Console.WriteLine("\nChoose export format txt(1)/csv(2)/xml(3)/: ");
string? choice = Console.ReadLine();

GridExporterFactory factory = new GridExporterFactory();
IGridExporter? export = factory.CreateExporter(choice);

if (export == null)
{
    Console.WriteLine("\n--ASCII_EXPORT--\n");
    gridA.Display();
}
else
{
    gridA.ExportDataGrid(export);
}

cfg.DisablePagination();
gridA.Display();


//ex5: receive si spend

var studentToUpdate = students.FirstOrDefault(s => s.StudentId == 11);
if (studentToUpdate != null)
{
    studentToUpdate.ReceiveMoney(4500,new DateTime(2024,1,2));
    studentToUpdate.SpendMoneySafe(1000,new DateTime(2024,1,2));


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
    Console.WriteLine("Student not found for update.");
}

Console.WriteLine();
Console.WriteLine("--Students_After_Updating_Student--");
gridA.Display();


cfg.EnablePagination(3);
Console.WriteLine("\n--Paginated View After Transactions--\n");
gridA.Display();


Console.WriteLine("\n-- GotToPage4 --\n");
gridA.GoToPage(99);
gridA.Display();

gridA.ExportDataGrid(new CsvExporter());