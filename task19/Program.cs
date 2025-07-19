using System;

public class Program
{
    public static void Main()
    {
        var serverThread = new ServerThread();
        for (int i = 1; i <= 5; i++)
        {
            var command = new TestCommand(i);
            serverThread.AddCommand(command);
        }

        //время на выполнение
        Thread.Sleep(7500);
        serverThread.HardStop();
        Console.WriteLine("Поток завершен.");
    }
}
