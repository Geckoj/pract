[AttributeUsage(AttributeTargets.Class)]
public class PluginLoadAttribute : Attribute
{
    public Type[] Dependencies { get; }

    public PluginLoadAttribute(params Type[] dependencies)
    {
        Dependencies = dependencies;
    }
}
public interface IPlugin
{
    void Execute();
}

[PluginLoad]
public class Plugin_1 : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("Plugin: 1");
    }
}

[PluginLoad]
public class Plugin_2 : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("Plugin: 2");
    }
}
[PluginLoad]
public class Plugin_3 : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("Plugin: 3");
    }
}

[PluginLoad(typeof(Plugin_2))]
public class Plugin_4 : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("Plugin: 4");
    }
}