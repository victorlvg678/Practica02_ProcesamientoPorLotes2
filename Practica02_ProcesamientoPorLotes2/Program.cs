using Practica02_ProcesamientoPorLotes2.Classes;
using Serilog;
public class Program
{
    public static void Main(string[] args)
    {
        using var log = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        Log.Logger = log;

        string folderPath = args.Length > 2 ? args[0] : "C:\\Temp\\BatchFiles";
        string extension = args.Length > 2 ? args[1] : ".txt";
        string savePath = args.Length > 2 ? args[2] : "C:\\Temp\\BatchFilesProcessed";

        Log.Information($"{Guid.NewGuid()} - Creating batch file processor");
        var batchFileProcessor = new BatchFileProcessor(folderPath, extension, savePath, (file) =>
        {
            var newFile = FileDataTransformer.Process(file);
            newFile.Name = $"Transformed_{file.Name}";
            file.Save();
        });

        Log.Information($"{Guid.NewGuid()} - Starting batch file processing");
        batchFileProcessor.GetFiles();
        batchFileProcessor.LoadFiles();
        batchFileProcessor.ProcessFiles();
        Log.Information($"{Guid.NewGuid()} - Files batch processing over");
    }
}

