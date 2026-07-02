using System;

namespace PluginBase;

public interface ICommand
{
    void Execute();
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PluginLoadAttribute : Attribute
{
    public string PluginName { get; }
    public string[] Dependencies { get; }

    public PluginLoadAttribute(string pluginName, params string[] dependencies)
    {
        PluginName = pluginName;
        Dependencies = dependencies ?? Array.Empty<string>();
    }
}
