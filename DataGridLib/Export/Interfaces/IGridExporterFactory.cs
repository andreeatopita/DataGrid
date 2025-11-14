namespace DataGridLib.Export.Interfaces;

public interface IGridExporterFactory
{
    IGridExporter? CreateExporter(string? extension);
}