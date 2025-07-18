class Program
{
    static void Main(string[] args)
    {
        string pluginsPath = Path.Combine(Directory.GetCurrentDirectory());

        var loader = new PluginLoader();
        try
        {
            loader.LoadAndExecutePlugins(pluginsPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки плагинов: {ex.Message}");
        }
    }
}