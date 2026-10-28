using Refresher.Core.Patching.Pipelines;

namespace Refresher.Core.Storage;

/// <summary>
/// Stores used inputs in pipelines for use in later sessions of Refresher.
/// </summary>
public static class PreviousInputStorage
{
    private const string ApplicationDirectory = "Refresher";
    private const string FileName = "inputs.ini";
    private static readonly string DirectoryPath;
    private static readonly string FilePath;
    
    public static UserEnteredInputs? Inputs;
    
    static PreviousInputStorage()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        DirectoryPath = Path.Combine(appData, ApplicationDirectory);
        FilePath = Path.Combine(appData, ApplicationDirectory, FileName);
    }

    public static void Read()
    {
        if (!File.Exists(FilePath))
            return;
        
        using FileStream stream = File.OpenRead(FilePath);
        using StreamReader reader = new(stream);

        Inputs = UserEnteredInputs.FromIni(reader);
    }

    public static void Write(UserEnteredInputs inputs)
    {
        Inputs = inputs;

        Directory.CreateDirectory(DirectoryPath);
        File.WriteAllText(FilePath, inputs.ToIni());
    }
}