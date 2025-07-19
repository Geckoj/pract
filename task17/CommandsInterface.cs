using System.Collections.Concurrent;
using System.Threading;

public interface ICommand
{
    void Execute();
}

public abstract class ControlCommand : ICommand
{
    protected readonly ServerThread ServerThread;

    protected ControlCommand(ServerThread serverThread)
    {
        ServerThread = serverThread;
    }

    public abstract void Execute();
}

public class HardStop : ControlCommand
{
    public HardStop(ServerThread serverThread) : base(serverThread) { }

    public override void Execute()
    {
        if (Thread.CurrentThread != ServerThread.WorkerThread)
            throw new InvalidOperationException("HardStop must execute in target thread");
        ServerThread.RequestHardStop();
    }
}

public class SoftStop : ControlCommand
{
    public SoftStop(ServerThread serverThread) : base(serverThread) { }

    public override void Execute()
    {
        if (Thread.CurrentThread != ServerThread.WorkerThread)
            throw new InvalidOperationException("SoftStop must execute in target thread");
        ServerThread.RequestSoftStop();
    }
}