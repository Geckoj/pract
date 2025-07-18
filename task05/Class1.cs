using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class ClassAnalyzer
{
    private readonly Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }
    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Select(m => m.Name);
    }
    public IEnumerable<string> GetMethodParams(string methodname)
    {
        var method = _type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                          .FirstOrDefault(m => m.Name == methodname);

        if (method == null)
            return Enumerable.Empty<string>();

        var parameters = method.GetParameters()
            .Select(p => $"{p.ParameterType.Name} {p.Name}");

        return parameters.Append($"returns: {method.ReturnType.Name}");
    }
    public IEnumerable<string> GetAllFields()
    {
        return _type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Select(f => f.Name);
    }
    public IEnumerable<string> GetProperties()
    {
        return _type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => p.Name);
    }
    public bool HasAttribute<T>() where T : Attribute
    {
        return _type.GetCustomAttributes(typeof(T), false).Any();
    }
}