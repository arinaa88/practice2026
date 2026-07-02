using System;
using System.IO;
using System.Reflection;
using CommandLib;

namespace CommandRunner;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Запуск динамического загрузчика команд...");

        string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");

        if (!File.Exists(dllPath))
        {
            dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "FileSystemCommands", "bin", "Debug", "net8.0", "FileSystemCommands.dll");
        }

        if (!File.Exists(dllPath))
        {
            Console.WriteLine($"Ошибка: Файл dll не найден по пути {dllPath}");
            return;
        }

        try
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);

            string demoDir = Path.Combine(Path.GetTempPath(), "RunnerDemoDir");
            Directory.CreateDirectory(demoDir);
            File.WriteAllText(Path.Combine(demoDir, "doc1.txt"), "Контент текстового файла");
            File.WriteAllText(Path.Combine(demoDir, "image.png"), "Данные картинки");

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    ICommand? command = null;

                    if (type.Name == "DirectorySizeCommand")
                    {
                        command = Activator.CreateInstance(type, demoDir) as ICommand;
                    }
                    else if (type.Name == "FindFilesCommand")
                    {
                        command = Activator.CreateInstance(type, demoDir, "*.txt") as ICommand;
                    }

                    command?.Execute();
                }
            }

            Directory.Delete(demoDir, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при динамической загрузке: {ex.Message}");
        }

        Console.ReadLine();
    }
}
