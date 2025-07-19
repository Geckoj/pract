using System;
using System.Collections.Concurrent;
using System.Threading;

public class ServerThread : IDisposable
{
    private readonly BlockingCollection<ICommand> _commandQueue = new();
    private readonly CancellationTokenSource _cts = new();
    private volatile bool _softStopRequested;
    private volatile bool _hardStopRequested;
    public Thread WorkerThread { get; }

    public ServerThread()
    {
        WorkerThread = new Thread(ProcessCommands) { IsBackground = true };
    }

    public void Start() => WorkerThread.Start();

    public void EnqueueCommand(ICommand command)
    {
        if (!_hardStopRequested && !_softStopRequested)
        {
            _commandQueue.Add(command);
        }
    }

    private void ProcessCommands()
    {
        try
        {
            while (true)
            {
                if (_hardStopRequested) return;

                ICommand command;
                if (_softStopRequested)
                {
                    if (!_commandQueue.TryTake(out command, 50))
                        return;
                }
                else
                {
                    command = _commandQueue.Take(_cts.Token);
                }

                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }

                if (_hardStopRequested) return;
            }
        }
        catch (OperationCanceledException) { }
    }

    internal void RequestHardStop()
    {
        _hardStopRequested = true;
        _cts.Cancel();
    }

    internal void RequestSoftStop()
    {
        _softStopRequested = true;
    }

    public void Dispose()
    {
        RequestHardStop();
        if (WorkerThread.IsAlive)
            WorkerThread.Join(1000);
        _cts.Dispose();
        _commandQueue.Dispose();
        GC.SuppressFinalize(this);
    }
}