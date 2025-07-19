using System;
using System.Collections.Concurrent;
using System.Threading;

public class CommandScheduler
{
    private readonly BlockingCollection<ICommand> _commandQueue = new();
    private bool _isRunning = true;

    public void ScheduleCommand(ICommand command)
    {
        _commandQueue.Add(command);
    }
    public void ExecuteCommands()
    {
        while (_isRunning)
        {
            if (_commandQueue.TryTake(out ICommand command))
            {
                while (!command.IsComplete())
                {
                    command.Execute();
                    Thread.Sleep(500);
                }
            }
        }
    }
    public void Stop()
    {
        _isRunning = false;
    }
}
