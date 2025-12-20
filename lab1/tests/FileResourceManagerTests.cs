using Lab1;
using Xunit;

namespace Lab1.Tests;

public class FileResourceManagerTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly List<string> _testFiles;

    public FileResourceManagerTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "Lab1FileManagerTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
        _testFiles = new List<string>();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenFilePathIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new FileResourceManager(""));
        Assert.Throws<ArgumentException>(() => new FileResourceManager("   "));
        Assert.Throws<ArgumentException>(() => new FileResourceManager(null!));
    }

    [Fact]
    public void OpenForWriting_ShouldCreateFile_WhenFileDoesNotExist()
    {
        var filePath = Path.Combine(_testDirectory, "write_test.txt");
        _testFiles.Add(filePath);
        using var manager = new FileResourceManager(filePath, FileMode.Create);
        manager.OpenForWriting();
        manager.WriteLine("Test line");
        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void WriteLine_ShouldWriteTextToFile()
    {
        var filePath = Path.Combine(_testDirectory, "writeline_test.txt");
        _testFiles.Add(filePath);
        using var manager = new FileResourceManager(filePath, FileMode.Create);
        manager.OpenForWriting();
        manager.WriteLine("First line");
        manager.WriteLine("Second line");
        var content = File.ReadAllText(filePath);
        Assert.Contains("First line", content);
        Assert.Contains("Second line", content);
    }

    [Fact]
    public void WriteLine_ShouldThrowInvalidOperationException_WhenNotOpenedForWriting()
    {
        var filePath = Path.Combine(_testDirectory, "writeline_error_test.txt");
        _testFiles.Add(filePath);
        using var manager = new FileResourceManager(filePath);
        Assert.Throws<InvalidOperationException>(() => manager.WriteLine("Test"));
    }

    [Fact]
    public void OpenForReading_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        var filePath = Path.Combine(_testDirectory, "nonexistent.txt");
        using var manager = new FileResourceManager(filePath);
        Assert.Throws<FileNotFoundException>(() => manager.OpenForReading());
    }

    [Fact]
    public void ReadAllText_ShouldReturnFileContent()
    {
        var filePath = Path.Combine(_testDirectory, "read_test.txt");
        _testFiles.Add(filePath);
        File.WriteAllText(filePath, "Line 1\nLine 2\nLine 3");
        using var manager = new FileResourceManager(filePath);
        manager.OpenForReading();
        var content = manager.ReadAllText();
        Assert.Contains("Line 1", content);
        Assert.Contains("Line 2", content);
        Assert.Contains("Line 3", content);
    }

    [Fact]
    public void ReadAllText_ShouldThrowInvalidOperationException_WhenNotOpenedForReading()
    {
        var filePath = Path.Combine(_testDirectory, "read_error_test.txt");
        _testFiles.Add(filePath);
        File.WriteAllText(filePath, "Test");
        using var manager = new FileResourceManager(filePath);
        Assert.Throws<InvalidOperationException>(() => manager.ReadAllText());
    }

    [Fact]
    public void AppendText_ShouldAppendTextToFile()
    {
        var filePath = Path.Combine(_testDirectory, "append_test.txt");
        _testFiles.Add(filePath);
        File.WriteAllText(filePath, "Original text\n");
        using var manager = new FileResourceManager(filePath);
        manager.AppendText("Appended text");
        var content = File.ReadAllText(filePath);
        Assert.Contains("Original text", content);
        Assert.Contains("Appended text", content);
    }

    [Fact]
    public void AppendText_ShouldCreateFile_WhenFileDoesNotExist()
    {
        var filePath = Path.Combine(_testDirectory, "append_create_test.txt");
        _testFiles.Add(filePath);
        using var manager = new FileResourceManager(filePath, FileMode.OpenOrCreate);
        manager.AppendText("New text");
        Assert.True(File.Exists(filePath));
        var content = File.ReadAllText(filePath);
        Assert.Contains("New text", content);
    }

    [Fact]
    public void GetFileInfo_ShouldReturnFileInfo()
    {
        var filePath = Path.Combine(_testDirectory, "info_test.txt");
        _testFiles.Add(filePath);
        var testContent = "Test content for file info";
        File.WriteAllText(filePath, testContent);
        using var manager = new FileResourceManager(filePath);
        var fileInfo = manager.GetFileInfo();
        Assert.NotNull(fileInfo);
        Assert.True(fileInfo.Exists);
        Assert.True(fileInfo.Length > 0);
    }

    [Fact]
    public void GetFileInfo_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        var filePath = Path.Combine(_testDirectory, "info_nonexistent.txt");
        using var manager = new FileResourceManager(filePath);
        Assert.Throws<FileNotFoundException>(() => manager.GetFileInfo());
    }

    [Fact]
    public void Dispose_ShouldReleaseResources()
    {
        var filePath = Path.Combine(_testDirectory, "dispose_test.txt");
        _testFiles.Add(filePath);
        var manager = new FileResourceManager(filePath, FileMode.Create);
        manager.OpenForWriting();
        manager.WriteLine("Test");
        manager.Dispose();
        Assert.Throws<ObjectDisposedException>(() => manager.WriteLine("Another test"));
    }

    [Fact]
    public void Dispose_ShouldThrowObjectDisposedException_AfterDisposal()
    {
        var filePath = Path.Combine(_testDirectory, "dispose_error_test.txt");
        _testFiles.Add(filePath);
        var manager = new FileResourceManager(filePath);
        manager.Dispose();
        Assert.Throws<ObjectDisposedException>(() => manager.OpenForWriting());
        Assert.Throws<ObjectDisposedException>(() => manager.OpenForReading());
        Assert.Throws<ObjectDisposedException>(() => manager.GetFileInfo());
    }

    [Fact]
    public void UsingStatement_ShouldAutomaticallyDispose()
    {
        var filePath = Path.Combine(_testDirectory, "using_test.txt");
        _testFiles.Add(filePath);
        using (var manager = new FileResourceManager(filePath, FileMode.Create))
        {
            manager.OpenForWriting();
            manager.WriteLine("Test line");
        }
        Assert.True(File.Exists(filePath));
        var content = File.ReadAllText(filePath);
        Assert.Contains("Test line", content);
    }

    [Fact]
    public void WriteLine_ShouldFlushImmediately()
    {
        var filePath = Path.Combine(_testDirectory, "flush_test.txt");
        _testFiles.Add(filePath);
        using var manager = new FileResourceManager(filePath, FileMode.Create);
        manager.OpenForWriting();
        manager.WriteLine("Flush test");
        var content = File.ReadAllText(filePath);
        Assert.Contains("Flush test", content);
    }

    public void Dispose()
    {
        foreach (var file in _testFiles)
        {
            try
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
            catch { }
        }
        try
        {
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }
        catch { }
    }
}
