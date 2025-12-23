// See https://aka.ms/new-console-template for more information

using BackofficeConsole.Commands;
using BackofficeConsole.DataAccess;
using BackofficeConsole.Domain.Repositories;
using BackofficeConsole.Services;
using BackofficeConsole.Services.Contracts;
using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

//container cand sa distruga ob,daca sa le refoloseasca sau sa creeze altele noi

//creez builder pentru aplicatia console folosind Cocona
Cocona.Builder.CoconaAppBuilder builder = CoconaApp.CreateBuilder(args);
//builder: config si pentru di  de servicii

//incarcare setari din appsettings.json
builder.Configuration.AddJsonFile("appsettings.json");

//connection string din app settings
string cs = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string Default not found.");
//cs pt DataAccess

//data access, sp e service provider, contine toate serviciile inregistrate 
builder.Services.AddSingleton<IAccountRepository>(sp => new AccountSqlRepository(cs));
builder.Services.AddSingleton<IStudentReadRepository>(sp => new StudentSqlRepository(cs));
builder.Services.AddSingleton<IStudentWriteRepository>(sp => new StudentSqlRepository(cs));
builder.Services.AddSingleton<IReportRepository>(sp => new ReportSqlRepository(cs));

//servicii , per executie de comanda
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IReportService, ReportService>();
//cand are nevie de IStudentService , ii da StudentService, iar pt StudentService va inject automat IStudentReadRepository si IStudentWriteRepository
//cand cnv cere iaccountservice, va primi AccountService cu repo injectat

//commands
builder.Services.AddTransient<StudentCommands>();
builder.Services.AddTransient<AccountCommands>();
builder.Services.AddTransient<ReportCommands>();
//de fiecare data cand e nevoie de comanda, se creeaza o noua instanta

//ia config, servicii, argumentele din linia de comanda
CoconaApp app = builder.Build();

//adaugare subcomenzi, 
app.AddSubCommand("students", x => x.AddCommands<StudentCommands>());
app.AddSubCommand("accounts", x => x.AddCommands<AccountCommands>());
app.AddSubCommand("reports", x => x.AddCommands<ReportCommands>());

app.Run();