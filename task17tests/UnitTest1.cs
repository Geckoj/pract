using System;
using System.Threading;
using Xunit;

public class ServerThreadTests : IDisposable
{
    private readonly ServerThread _serverThread;

    public ServerThreadTests()
    {
        _serverThread = new ServerThread();
        _serverThread.Start();
    }

    public void Dispose() => _serverThread.Dispose();

    [Fact]
    public void SoftStop_CompletesAllCommandsInQueue()
    {
        int counter = 0;
        var signal = new ManualResetEventSlim();

        _serverThread.EnqueueCommand(new TestCommand(() =>
        {
            Thread.Sleep(100);
            Interlocked.Increment(ref counter);
        }));
        _serverThread.EnqueueCommand(new TestCommand(() => Interlocked.Increment(ref counter)));
        _serverThread.EnqueueCommand(new SoftStop(_serverThread));
        _serverThread.EnqueueCommand(new TestCommand(() =>
        {
            Interlocked.Increment(ref counter);
            signal.Set();
        }));

        _serverThread.WorkerThread.Join(500);

        Assert.Equal(3, counter);
        Assert.True(signal.IsSet);
        Assert.False(_serverThread.WorkerThread.IsAlive);
    }

    [Fact]
    public void SoftStop_DoesNotExecuteCommandsAfter()
    {
        int counter = 0;
        var signal = new ManualResetEventSlim();
        _serverThread.EnqueueCommand(new TestCommand(() => Interlocked.Increment(ref counter)));
        _serverThread.EnqueueCommand(new SoftStop(_serverThread));

        _serverThread.WorkerThread.Join(500);
        Assert.False(_serverThread.WorkerThread.IsAlive);
        _serverThread.EnqueueCommand(new TestCommand(() =>
        {
            Interlocked.Increment(ref counter);
            signal.Set();
        }));

        Thread.Sleep(100);

        Assert.Equal(1, counter);
        Assert.False(signal.IsSet);
    }

    [Fact]
    public void HardStop_StopsAfterCurrentCommand()
    {
        int counter = 0;
        var longCommandSignal = new ManualResetEventSlim();
        var nextCommandSignal = new ManualResetEventSlim();

        _serverThread.EnqueueCommand(new TestCommand(() =>
        {
            longCommandSignal.Set();
            Thread.Sleep(100);
            Interlocked.Increment(ref counter);
        }));

        Assert.True(longCommandSignal.Wait(500));

        _serverThread.EnqueueCommand(new HardStop(_serverThread));
        _serverThread.EnqueueCommand(new TestCommand(() =>
        {
            Interlocked.Increment(ref counter);
            nextCommandSignal.Set();
        }));

        _serverThread.WorkerThread.Join(500);

        Assert.Equal(1, counter);
        Assert.False(nextCommandSignal.IsSet);
        Assert.False(_serverThread.WorkerThread.IsAlive);
    }

    [Fact]
    public void Commands_ThrowIfExecutedInWrongThread()
    {
        var hardStop = new HardStop(_serverThread);
        var softStop = new SoftStop(_serverThread);

        Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
        Assert.Throws<InvalidOperationException>(() => softStop.Execute());
    }

    private class TestCommand : ICommand
    {
        private readonly Action _action;
        public TestCommand(Action action) => _action = action;
        public void Execute() => _action();
    }
}