using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace task07;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    public DisplayNameAttribute(string displayName) => DisplayName = displayName;
}

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }
    public VersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }
}

[DisplayName("Пример класса")]
[Version(1, 0)]
public class SampleClass
{
    [DisplayName("Числовое свойство")]
    public int Number { get; set; }

    [DisplayName("Тестовый метод")]
    public void TestMethod() { }
}
public static class ReflectionHelper
{
    public static string PrintTypeInfo(Type type)
    {
        if (type == null) return string.Empty;

        var result = new List<string>();

        var classDisplay = type.GetCustomAttribute<DisplayNameAttribute>();
        if (classDisplay != null) result.Add($"Class: {classDisplay.DisplayName}");

        var classVersion = type.GetCustomAttribute<VersionAttribute>();
        if (classVersion != null) result.Add($"Version: {classVersion.Major}.{classVersion.Minor}");

        var props = type.GetProperties()
            .Select(p => new { p.Name, Attr = p.GetCustomAttribute<DisplayNameAttribute>() })
            .Where(p => p.Attr != null)
            .Select(p => $"Property: {p.Name} ({p.Attr!.DisplayName})");
        result.AddRange(props);

        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Select(m => new { m.Name, Attr = m.GetCustomAttribute<DisplayNameAttribute>() })
            .Where(m => m.Attr != null)
            .Select(m => $"Method: {m.Name} ({m.Attr!.DisplayName})");
        result.AddRange(methods);

        return string.Join("\n", result);
    }
}