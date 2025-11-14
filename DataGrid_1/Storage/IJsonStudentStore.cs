
namespace DataGrid_1.Storage;

public interface IJsonStudentStore
{
    List<Student> Load();
    bool Save(List<Student> students);
}