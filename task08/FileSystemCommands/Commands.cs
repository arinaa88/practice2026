using System;
using System.IO;
using System.Linq;
using CommandLib;

namespace FileSystemCommands;

public class DirectorySizeCommand : ICommand
{
    private readonly string _directoryPath;
    public long CalculatedSize { get; private set; }

    public DirectorySizeCommand(string directoryPath)
    {
        _directoryPath = directoryPath;
    }

    public void Execute()
    {
        if (!Directory.Exists(_directoryPath))
        {
            Console.WriteLine($"Каталог {_directoryPath} не существует.");
            return;
        }

        CalculatedSize = Directory.GetFiles(_directoryPath, "*", SearchOption.AllDirectories)
                                  .Select(f => new FileInfo(f).Length)
                                  .Sum();

        Console.WriteLine($"Размер каталога '{_directoryPath}': {CalculatedSize} байт.");
    }
}

public class FindFilesCommand : ICommand
{
    private readonly string _directoryPath;
    private readonly string _searchPattern;
    public string[] FoundFiles { get; private set; } = Array.Empty<string>();

    public FindFilesCommand(string directoryPath, string searchPattern)
    {
        _directoryPath = directoryPath;
        _searchPattern = searchPattern;
    }

    public void Execute()
    {
        if (!Directory.Exists(_directoryPath))
        {
            Console.WriteLine($"Каталог {_directoryPath} не существует.");
            return;
        }

        FoundFiles = Directory.GetFiles(_directoryPath, _searchPattern, SearchOption.TopDirectoryOnly);

        Console.WriteLine($"Найдено файлов по маске '{_searchPattern}' в каталоге '{_directoryPath}':");
        foreach (var file in FoundFiles)
        {
            Console.WriteLine(Path.GetFileName(file));
        }
    }
}
