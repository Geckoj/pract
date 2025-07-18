using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Runtime.Loader;

public interface ICalculator
{
    int Add(int a, int b);
    int Minus(int a, int b);
    int Mul(int a, int b);
    int Div(int a, int b);
}

public class CalcBuilder
{
    public static ICalculator GetCalculator()
    {
        string code = @"
        public class Calculator : ICalculator
        {
            public int Add(int a, int b) => a + b;
            public int Minus(int a, int b) => a - b;
            public int Mul(int a, int b) => a * b;
            public int Div(int a, int b) => a / b;
        }";

        string assemblyName = "DynamicCalc_" + Guid.NewGuid().ToString();


        var tree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("DynamicCalc")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location)
            )
            .AddSyntaxTrees(tree);

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            Console.WriteLine("Compilation failed:");
            foreach (var diagnostic in result.Diagnostics)
                Console.WriteLine(diagnostic.ToString());
            return null;
        }

        ms.Seek(0, SeekOrigin.Begin);
        var loadContext = new AssemblyLoadContext(assemblyName, isCollectible: true);
        var assembly = loadContext.LoadFromStream(ms);
        var type = assembly.GetTypes().FirstOrDefault(t => typeof(ICalculator).IsAssignableFrom(t));

        if (type == null)
        {
            Console.WriteLine("No suitable type found.");
            return null;
        }

        return (ICalculator)Activator.CreateInstance(type)!;
    }
}
