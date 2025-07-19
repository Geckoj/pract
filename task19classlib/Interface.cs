public interface ICommand
{
    void Execute();
    bool IsComplete();
}

public class TestCommand : ICommand
{
    private int _id;
    private int _counter = 0;
    private const int _maxExecutions = 3;

    public TestCommand(int id)
    {
        _id = id;
    }

    public void Execute()
    {
        Console.WriteLine($"Поток {_id} вызов {_counter + 1}");
        _counter++;
    }

    public bool IsComplete()
    {
        return _counter >= _maxExecutions;
    }
}
