// See https://aka.ms/new-console-template for more information

using DataGrid_1;
using DataGrid_1.Formatting;
using DataGrid_1.Grids;
using System.Globalization;
using System.Text;
using DataGrid_1.Export;
using DataGridLib.Export;
using DataGridLib.Export.Interfaces;
using DataGridLib.DataGrid;
using DataGrid_1.Repository;
using DataGridLib.Contracts;
using Microsoft.Extensions.Configuration;

Console.OutputEncoding = Encoding.UTF8;


Console.WriteLine("Choose data source: 1-JSON, 2 -SQL");
string? sourceChoice=Console.ReadLine();


// incarc datele din fisier 
string jsonPath = Path.Combine(AppContext.BaseDirectory, "Data","students.json");

var config= new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

string cs = config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string not found.");

Console.WriteLine($"JSON path: {jsonPath}");
Console.WriteLine($"JSON path: {cs}");


IRepository<Student> repo = sourceChoice switch
{
    "1" => new JsonRepository(jsonPath),  
    "2" => new SqlRepository(cs),
    _ => new JsonRepository(jsonPath), //default JSON
};

//incarcare studentii din repo
IReadOnlyList<Student> students = await repo.LoadAsync();

Console.WriteLine($"Loaded {students.Count} students from {repo.GetType().Name}");

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

GridDataSource<Student> dataSourceA = new GridDataSource<Student>(repo);
var gridA= new DataGrid<Student>(cfg, dataSourceA);

await gridA.DisplayAsync();
Console.WriteLine();


Console.WriteLine("--Students_Partial--");
await gridA.DisplayAsync("Id", "Name", "Active");
Console.WriteLine();

Console.WriteLine("--Students_Active_Only_LastName--");
cfg.Where(s => s.IsActive)
    .OrderBy(s => s.LastName);
await gridA.DisplayAsync();
Console.WriteLine();



cfg.ResetQuery();
Console.WriteLine("--Students_Ordered_by_LastName--");
cfg.OrderBy(s => s.LastName);
await gridA.DisplayAsync();
Console.WriteLine();

//subset de randuri, skip : sare peste primele n randuri
cfg.ResetQuery(); //resetez filtrarea, ordonarea, skip, take
cfg.Skip(2);
Console.WriteLine("--Students_Skip_2--");
await gridA.DisplayAsync();
Console.WriteLine();


//take: ia primele n randuri
cfg.ResetQuery(); //resetez filtrarea, ordonarea, skip, take
cfg.Take(5);
Console.WriteLine("--Students_A_Take_5--");
await gridA.DisplayAsync();
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


Console.WriteLine();
Console.WriteLine("--Students_After_Updating_Student--");
await gridA.DisplayAsync();


//EX 2 studenti inactivi
Console.WriteLine("\n--Students_Inactive--\n");
GridConfiguration<Student> inactiveCfg = new InactiveStudentsGrid().Build();
DataGrid<Student> inactiveGrid = new DataGrid<Student>(inactiveCfg, dataSourceA);
await inactiveGrid.DisplayAsync();

//EX 3 studenti cu account balance >= 200 
Console.WriteLine("\n--Students_Balance_Above_200_Default--\n");
GridConfiguration<Student> balanceCfg = new StudentsByBalanceGrid().Build();
DataGrid<Student> balanceGrid = new DataGrid<Student>(balanceCfg, dataSourceA);
await balanceGrid.DisplayAsync();

//EX 3 CULTURA
Console.WriteLine("\n--Students_Balance_Above_200_RO_Culture--\n");
GridConfiguration<Student> balanceCfgRO = new StudentsByBalanceGrid(new CultureInfo("ro-RO")).Build();
DataGrid<Student> balanceGridRO = new DataGrid<Student>(balanceCfgRO, dataSourceA);
await balanceGridRO.DisplayAsync();

//EX 3 sufix
Console.WriteLine("\n--Students_Balance_Above_200_Custom_Suffix--\n");
GridConfiguration<Student> balanceCfgCustom = new StudentsByBalanceGrid(new CultureInfo("fr-FR"), suffix: "Eur").Build();
DataGrid<Student> balanceGridCustom = new DataGrid<Student>(balanceCfgCustom, dataSourceA);
await balanceGridCustom.DisplayAsync();

//ex 3 cu ja
Console.WriteLine("\n--Students_Balance_Above_200_Custom_Prefix--\n");
GridConfiguration<Student> balanceCfgJP = new StudentsByBalanceGrid(new CultureInfo("ja-JP")).Build();
DataGrid<Student> balanceGridJP = new DataGrid<Student>(balanceCfgJP, dataSourceA);
await balanceGridJP.DisplayAsync();


//EX 5
Console.WriteLine("\n--Students_Recent_High_Value_Transactions--\n");
GridConfiguration<Student> recentHighCfg = new RecentHighValueTransactionsGrid(culture: new CultureInfo("ro-RO")).Build();
DataGrid<Student> recentHighGrid = new DataGrid<Student>(recentHighCfg, dataSourceA);
await recentHighGrid.DisplayAsync();


//export in format

//EX 4 export datagrid to csv or xml
Console.WriteLine("\nChoose export format txt(1)/csv(2)/xml(3)/: ");
string? choice = Console.ReadLine();

GridExporterFactory factory = new GridExporterFactory();
IGridExporter? export = factory.CreateExporter(choice);

if (export == null)
{
    Console.WriteLine("\n--ASCII_EXPORT--\n");
    await gridA.DisplayAsync();
}
else
{
    await gridA.ExportDataGridAsync(export);
}

await gridA.DisplayAsync();


//ex5: receive si spend

var studentToUpdate = students.FirstOrDefault(s => s.StudentId == 12);
if (studentToUpdate != null)
{
    studentToUpdate.ReceiveSafe(4500);
    studentToUpdate.SpendMoneySafe(1000);

    if(repo is JsonRepository jsonRepo)
    {
        await jsonRepo.SaveAsync(students);
        Console.WriteLine("Student updated and saved to JSON.");
    }
    else
    {
        Console.WriteLine("Save only supported for JsonRepository");
    }
}
else
{
    Console.WriteLine("Student not found for update.");
}

Console.WriteLine();
Console.WriteLine("--Students_After_Updating_Student--");
await gridA.DisplayAsync();


//EX 6 : paginare 

//pornesc paginarea :
cfg.EnablePagination(6);

Console.WriteLine("\n-- Page 1 (after EnablePagination) --\n");
await gridA.DisplayAsync();

Console.WriteLine("\n-- NextPage --\n");
gridA.Next();
await gridA.DisplayAsync();

Console.WriteLine("\n-- GotToPage5 --\n");
gridA.GoToPage(99);
await gridA.DisplayAsync();

Console.WriteLine("\n-- Previous Page --\n");
gridA.Previous();
await gridA.DisplayAsync();

Console.WriteLine("\n-- LastPage --\n");
gridA.Last();
await gridA.DisplayAsync();

Console.WriteLine("\n-- FirstPage --\n");
gridA.First();
await gridA.DisplayAsync();

//sa vad daca da export paginat 
await gridA.ExportDataGridAsync(new CsvExporter());

Console.WriteLine("\n-- ChangePageSize --\n");
gridA.ChangePageSize(3);
await gridA.DisplayAsync();

cfg.DisablePagination();
Console.WriteLine("\n-- Pagination Disabled --\n");
await gridA.DisplayAsync();
