using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PluginBase;

namespace PluginApp;

[PluginLoad("PluginC", "PluginB")]
public class CommandC : ICommand
{
    public void Execute() => Console.WriteLine("[PluginC] Выполнен: Зависимости удовлетворенны.");
}

[PluginLoad("PluginA")]
public class CommandA : ICommand
{
    public void Execute() => Console.WriteLine("[PluginA] Выполнен: Начальная инициализация базовых систем.");
}

[PluginLoad("PluginB", "PluginA")]
public class CommandB : ICommand
{
    public void Execute() => Console.WriteLine("[PluginB] Выполнен: Промежуточный модуль запущен.");
}


class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Запуск интеллектуальной системы плагинов");

        var pluginTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<PluginLoadAttribute>() != null && typeof(ICommand).IsAssignableFrom(t))
            .ToList();

        var pluginsMap = new Dictionary<string, Type>();
        var adjacencyList = new Dictionary<string, List<string>>();

        foreach (var type in pluginTypes)
        {
            var attr = type.GetCustomAttribute<PluginLoadAttribute>()!;
            pluginsMap[attr.PluginName] = type;
            adjacencyList[attr.PluginName] = attr.Dependencies.ToList();
        }

        try
        {
            List<string> sortedPluginNames = TopologicalSort(adjacencyList);

            Console.WriteLine("Определен безопасный порядок загрузки плагинов:");
            Console.WriteLine(string.Join(" -> ", sortedPluginNames) + "\n");

            Console.WriteLine("=== Инициализация и выполнение плагинов ===");
            foreach (var name in sortedPluginNames)
            {
                if (pluginsMap.TryGetValue(name, out Type? type))
                {
                    var instance = Activator.CreateInstance(type) as ICommand;
                    instance?.Execute();
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"\n[Критическая ошибка]: {ex.Message}");
        }

        Console.ReadLine();
    }

    private static List<string> TopologicalSort(Dictionary<string, List<string>> graph)
    {
        var result = new List<string>();
        var visited = new Dictionary<string, bool>(); 

        foreach (var node in graph.Keys)
        {
            if (!visited.ContainsKey(node))
            {
                Visit(node, graph, visited, result);
            }
        }

        return result;
    }

    private static void Visit(string node, Dictionary<string, List<string>> graph, Dictionary<string, bool> visited, List<string> result)
    {
        if (visited.TryGetValue(node, out bool isFullyVisited))
        {
            if (!isFullyVisited)
            {
                throw new InvalidOperationException("Обнаружена циклическая зависимость между плагинами! Загрузка невозможна.");
            }
            return;
        }

        visited[node] = false; 

        if (graph.TryGetValue(node, out var dependencies))
        {
            foreach (var dependency in dependencies)
            {
                if (graph.ContainsKey(dependency))
                {
                    Visit(dependency, graph, visited, result);
                }
            }
        }

        visited[node] = true; 
        result.Add(node); 
    }
}
