using System.Text;

namespace Lab1;

public class FileResourceManager : IDisposable
{
    private FileStream? _fileStream;
    private StreamWriter? _writer;
    private StreamReader? _reader;
    private bool _disposed = false;
    private readonly string _filePath;
    private readonly FileMode _fileMode;

    public FileResourceManager(string filePath, FileMode fileMode = FileMode.OpenOrCreate)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Путь к файлу не может быть пустым", nameof(filePath));
        }
        _filePath = filePath;
        _fileMode = fileMode;
    }

    public void OpenForWriting()
    {
        ThrowIfDisposed();
        try
        {
            _fileStream = new FileStream(_filePath, _fileMode, FileAccess.Write, FileShare.Read);
            _writer = new StreamWriter(_fileStream, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            throw new IOException($"Ошибка при открытии файла для записи: {ex.Message}", ex);
        }
    }

    public void OpenForReading()
    {
        ThrowIfDisposed();
        try
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {_filePath}");
            }
            _fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _reader = new StreamReader(_fileStream, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            throw new IOException($"Ошибка при открытии файла для чтения: {ex.Message}", ex);
        }
    }

    public void WriteLine(string text)
    {
        ThrowIfDisposed();
        if (_writer == null)
        {
            throw new InvalidOperationException("Файл не открыт для записи. Вызовите OpenForWriting()");
        }
        try
        {
            _writer.WriteLine(text);
            _writer.Flush();
        }
        catch (Exception ex)
        {
            throw new IOException($"Ошибка при записи в файл: {ex.Message}", ex);
        }
    }

    public string ReadAllText()
    {
        ThrowIfDisposed();
        if (_reader == null)
        {
            throw new InvalidOperationException("Файл не открыт для чтения. Вызовите OpenForReading()");
        }
        try
        {
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return _reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            throw new IOException($"Ошибка при чтении файла: {ex.Message}", ex);
        }
    }

    public void AppendText(string text)
    {
        ThrowIfDisposed();
        try
        {
            if (_writer == null)
            {
                using var fileStream = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                using var writer = new StreamWriter(fileStream, Encoding.UTF8);
                writer.Write(text);
                writer.Flush();
            }
            else
            {
                _writer.Write(text);
                _writer.Flush();
            }
        }
        catch (Exception ex)
        {
            throw new IOException($"Ошибка при добавлении текста в файл: {ex.Message}", ex);
        }
    }

    public FileInfo GetFileInfo()
    {
        ThrowIfDisposed();
        try
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {_filePath}");
            }
            return new FileInfo(_filePath);
        }
        catch (Exception ex)
        {
            throw new IOException($"Ошибка при получении информации о файле: {ex.Message}", ex);
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(FileResourceManager));
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _writer?.Dispose();
                _reader?.Dispose();
                _fileStream?.Dispose();
                _writer = null;
                _reader = null;
                _fileStream = null;
            }
            _disposed = true;
        }
    }

    ~FileResourceManager()
    {
        Dispose(false);
    }
}
