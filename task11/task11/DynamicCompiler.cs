using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11;

public interface ICalculator
{
    int Add(int a, int b);
    int Minus(int a, int b);
    int Mul(int a, int b);
    int Div(int a, int b);
}

public static class DynamicCompiler
{
    public static ICalculator CreateCalculatorInstance()
    {
        string classCode = @"
            using System;
            using task11;

            public class Calculator : ICalculator
            {
                public int Add(int a, int b) => a + b;
                public int Minus(int a, int b) => a - b;
                public int Mul(int a, int b) => a * b;
                public int Div(int a, int b) => a / b;
            }";

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classCode);

        string assemblyName = Path.GetRandomFileName();

        MetadataReference[] references = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location)
        };

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

            string errors = string.Join("\n", failures.Select(f => f.GetMessage()));
            throw new InvalidOperationException($"Ошибка динамической компиляции кода:\n{errors}");
        }

        ms.Seek(0, SeekOrigin.Begin);
        Assembly assembly = Assembly.Load(ms.ToArray());

        Type calculatorType = assembly.GetType("Calculator")
            ?? throw new TypeLoadException("Не удалось найти динамический класс Calculator.");

        return (ICalculator)Activator.CreateInstance(calculatorType)!;
    }
}
