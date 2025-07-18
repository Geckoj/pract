using FileSystemCommands;
using Microsoft.VisualStudio.TestPlatform.Utilities;
public class FileSystemCommandsTests
{
    private readonly StringWriter _consoleOutput;
    private readonly TextWriter _originalOutput;

    public FileSystemCommandsTests()
    {
        _consoleOutput = new StringWriter();
        _originalOutput = Console.Out;
        Console.SetOut(_consoleOutput);
    }

    public void Dispose()
    {
        Console.SetOut(_originalOutput);
        _consoleOutput.Dispose();
    }


    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        var command = new DirectorySizeCommand(testDir);
        command.Execute(); // Проверяем, что не возникает исключений

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute(); // Должен найти 1 файл

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void DirectorySizeCommand_ConsoleOutput()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World!");
        long expectedSize = 5 + 6;

        var command = new DirectorySizeCommand(testDir);
        command.Execute();
        string output = _consoleOutput.ToString();

        Assert.Contains($"Directory size: {expectedSize} bytes", output);
        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ConsoleOutput()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute(); // Должен найти 1 файл
        string output = _consoleOutput.ToString();

        Assert.Contains("Found 1 files:", output);
        Assert.Contains("file1.txt", output);
        Assert.DoesNotContain("file2.log", output);

        Directory.Delete(testDir, true);
    }
}