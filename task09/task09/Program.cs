using System;
using System.IO;
using System.Reflection;
using System.Linq;

namespace task09;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Ошибка: Не указан путь к динамической библиотеке (DLL).");
            Console.WriteLine("Использование: task09 <путь_к_файлу.dll>");
            return;
        }

        string dllPath = args[0];

        if (!File.Exists(dllPath))
        {
            Console.WriteLine($"Ошибка: Файл по пути '{dllPath}' не найден.");
            return;
        }

        try
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);
            Console.WriteLine($"Анализ библиотеки: {assembly.GetName().Name}\n");

            var classes = assembly.GetTypes().Where(t => t.IsClass);

            foreach (Type type in classes)
            {
                Console.WriteLine($"Класс: {type.FullName}");

                var classAttributes = type.GetCustomAttributes(false);
                if (classAttributes.Any())
                {
                    Console.WriteLine("  Атрибуты класса:");
                    foreach (var attr in classAttributes)
                    {
                        Console.WriteLine($"    - [{attr.GetType().Name}]");
                    }
                }

                var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                Console.WriteLine("  Конструкторы:");
                foreach (var ctor in constructors)
                {
                    Console.WriteLine($"    - {type.Name}");
                    var parameters = ctor.GetParameters();
                    if (parameters.Any())
                    {
                        Console.WriteLine("      Параметры:");
                        foreach (var param in parameters)
                        {
                            Console.WriteLine($"        {param.ParameterType.Name} {param.Name}");
                        }
                    }
                }

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                                  .Where(m => !m.IsSpecialName); 

                Console.WriteLine("  Методы:");
                foreach (var method in methods)
                {
                    Console.WriteLine($"    - {method.ReturnType.Name} {method.Name}");

                    var methodAttributes = method.GetCustomAttributes(false);
                    if (methodAttributes.Any())
                    {
                        Console.WriteLine("      Атрибуты метода:");
                        foreach (var attr in methodAttributes)
                        {
                            Console.WriteLine($"        - [{attr.GetType().Name}]");
                        }
                    }

                    var parameters = method.GetParameters();
                    if (parameters.Any())
                    {
                        Console.WriteLine("      Параметры метода:");
                        foreach (var param in parameters)
                        {
                            Console.WriteLine($"        {param.ParameterType.Name} {param.Name}");
                        }
                    }
                }
                Console.WriteLine(new string('-', 50));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка при извлечении метаданных: {ex.Message}");
        }
    }
}

