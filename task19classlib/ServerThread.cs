using System;
using System.Threading;

public class ServerThread
{
    private readonly CommandScheduler _scheduler;
    private readonly Thread _thread;

    public ServerThread()
    {
        _scheduler = new CommandScheduler();
        _thread = new Thread(() => _scheduler.ExecuteCommands());
        _thread.Start();
    }

    public void AddCommand(ICommand command)
    {
        _scheduler.ScheduleCommand(command);
    }

    public void HardStop()
    {
        _scheduler.Stop();
        _thread.Join();
    }
}
