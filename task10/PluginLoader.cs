using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public class PluginLoader
{
    public void LoadAndExecutePlugins(string pluginsPath)
    {
        var assemblies = LoadAssemblies(pluginsPath);
        var pluginTypes = FindPluginTypes(assemblies);
        var (graph, nodeToType) = BuildDependencyGraph(pluginTypes);
        var loadOrder = TopologicalSort(graph);
        ExecutePlugins(loadOrder, nodeToType);
    }

    private List<Assembly> LoadAssemblies(string path)
    {
        var assemblies = new List<Assembly>();
        foreach (var dll in Directory.GetFiles(path, "*.dll"))
        {
            try
            {
                assemblies.Add(Assembly.LoadFrom(dll));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки {Path.GetFileName(dll)}: {ex.Message}");
            }
        }
        return assemblies;
    }

    private List<Type> FindPluginTypes(List<Assembly> assemblies)
    {
        var pluginTypes = new List<Type>();
        foreach (var assembly in assemblies)
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.GetCustomAttribute<PluginLoadAttribute>() != null)
                    {
                        pluginTypes.Add(type);
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"Ошибка загрузки типов из {assembly.GetName().Name}: {ex.LoaderExceptions[0]?.Message}");
            }
        }
        return pluginTypes;
    }

    private (Dictionary<string, List<string>>, Dictionary<string, Type>)
        BuildDependencyGraph(List<Type> pluginTypes)
    {
        var graph = new Dictionary<string, List<string>>();
        var nodeToType = new Dictionary<string, Type>();
        var typeToNode = new Dictionary<Type, string>();

        foreach (var type in pluginTypes)
        {
            var node = type.FullName;
            nodeToType[node] = type;
            typeToNode[type] = node;
            graph[node] = new List<string>();
        }

        foreach (var type in pluginTypes)
        {
            var attr = type.GetCustomAttribute<PluginLoadAttribute>();
            if (attr?.Dependencies != null)
            {
                foreach (var dependency in attr.Dependencies)
                {
                    if (typeToNode.TryGetValue(dependency, out var depNode))
                    {
                        graph[typeToNode[type]].Add(depNode);
                    }
                }
            }
        }

        return (graph, nodeToType);
    }

    private List<string> TopologicalSort(Dictionary<string, List<string>> graph)
    {
        var inDegree = new Dictionary<string, int>();
        var queue = new Queue<string>();
        var result = new List<string>();

        foreach (var node in graph.Keys)
        {
            inDegree[node] = 0;
        }
        foreach (var dependencies in graph.Values)
        {
            foreach (var dep in dependencies)
            {
                inDegree[dep]++;
            }
        }
        foreach (var node in graph.Keys)
        {
            if (inDegree[node] == 0)
            {
                queue.Enqueue(node);
            }
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);

            foreach (var neighbor in graph[current])
            {
                inDegree[neighbor]--;
                if (inDegree[neighbor] == 0)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (result.Count != graph.Count)
        {
            throw new Exception("Обнаружен цикл в зависимостях плагинов!");
        }

        return result;
    }

    private void ExecutePlugins(List<string> loadOrder, Dictionary<string, Type> nodeToType)
    {
        var pluginInstances = new Dictionary<string, IPlugin>();

        foreach (var node in loadOrder)
        {
            var type = nodeToType[node];
            var plugin = (IPlugin)Activator.CreateInstance(type);
            pluginInstances[node] = plugin;

            Console.WriteLine($"Загружен плагин: {type.Name}");
            plugin.Execute();
        }
    }
}