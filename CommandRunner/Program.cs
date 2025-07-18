using System.Reflection;



class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: CommandRunner <path-to-dll>");
            return;
        }

        string assemblyPath = args[0];

        try
        {
            AnalyzeAssembly(assemblyPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    static void AnalyzeAssembly(string assemblyPath)
    {
        Assembly assembly = Assembly.LoadFrom(assemblyPath);
        Console.WriteLine($"Assembly: {assembly.GetName().Name}\n");

        foreach (Type type in assembly.GetTypes().Where(t => t.IsPublic))
        {
            PrintTypeInfo(type);
        }
    }

    static void PrintTypeInfo(Type type)
    {
        Console.WriteLine($"\n===== Type: {type.FullName} =====");

        PrintAttributes("Class Attributes", type.GetCustomAttributes());
        PrintConstructors(type);
        PrintMethods(type);
        PrintProperties(type);
    }
    static void PrintAttributes(string title, IEnumerable<Attribute> attributes)
    {
        var attributeList = attributes.ToList();
        if (!attributeList.Any()) return;

        Console.WriteLine($"{title}:");
        foreach (Attribute attr in attributeList)
        {
            switch (attr)
            {
                case DisplayNameAttribute displayNameAttr:
                    Console.WriteLine($"  DisplayName: {displayNameAttr.DisplayName}");
                    break;
                case VersionAttribute versionAttr:
                    Console.WriteLine($"  Version: {versionAttr.Major}.{versionAttr.Minor}");
                    break;
                default:
                    Console.WriteLine($"  {attr.GetType().Name}");
                    break;
            }
        }
    }

    static void PrintConstructors(Type type)
    {
        var constructors = type.GetConstructors(
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static
        );

        if (!constructors.Any()) return;

        Console.WriteLine("Constructors:");
        foreach (ConstructorInfo ctor in constructors)
        {
            if (ctor.IsStatic)
            {
                Console.WriteLine($"  static {type.Name}()");
                continue;
            }
            string parameters = string.Join(", ", ctor.GetParameters()
                .Select(p => $"{GetTypeName(p.ParameterType)} {p.Name}"));

            Console.WriteLine($"  {type.Name}({parameters})");
            PrintAttributes("    Constructor Attributes", ctor.GetCustomAttributes());
        }
    }

    static void PrintMethods(Type type)
    {
        var methods = type.GetMethods(
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly
        ).Where(m => !m.IsSpecialName);

        if (!methods.Any()) return;

        Console.WriteLine("Methods:");
        foreach (MethodInfo method in methods)
        {
            string staticModifier = method.IsStatic ? "static " : "";
            string returnType = GetTypeName(method.ReturnType);
            string parameters = string.Join(", ", method.GetParameters()
                .Select(p => $"{GetTypeName(p.ParameterType)} {p.Name}"));

            Console.WriteLine($"  {staticModifier}{returnType} {method.Name}({parameters})");
            PrintAttributes("    Method Attributes", method.GetCustomAttributes());
        }
    }

    static void PrintProperties(Type type)
    {
        var properties = type.GetProperties(
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static
        );

        if (!properties.Any()) return;

        Console.WriteLine("Properties:");
        foreach (PropertyInfo prop in properties)
        {
            Console.WriteLine($"  {GetTypeName(prop.PropertyType)} {prop.Name}");
            PrintAttributes("    Property Attributes", prop.GetCustomAttributes());
        }
    }

    static string GetTypeName(Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        string name = type.Name.Split('`')[0];
        string args = string.Join(", ", type.GetGenericArguments().Select(GetTypeName));
        return $"{name}<{args}>";
    }
}
