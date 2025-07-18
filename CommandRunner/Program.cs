using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: CommandRunner <command-dll> <command-name> [parameters]");
            return;
        }

        string dllPath = args[0];
        string commandName = args[1];

        try
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);

            Type commandType = null;
            foreach (var type in assembly.GetTypes())
            {
                if (type.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase)
                    && typeof(ICommand).IsAssignableFrom(type))
                {
                    commandType = type;
                    break;
                }
            }

            if (commandType == null)
            {
                Console.WriteLine($"Command '{commandName}' not found in {dllPath}");
                return;
            }
            object[] parameters = new object[args.Length - 2];
            for (int i = 2; i < args.Length; i++)
            {
                parameters[i - 2] = args[i];
            }
            var command = (ICommand)Activator.CreateInstance(commandType, parameters);
            command.Execute();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
