public interface ISpaceship
{
    void MoveForward();      // Движение вперед
    void Rotate(int angle);  // Поворот на угол (градусы)
    void Fire();             // Выстрел ракетой
    int Speed { get; }       // Скорость корабля
    int FirePower { get; }   // Мощность выстрела
    double x { get;}        // Координата X
    double y { get;}        // Координата Y
    double angle { get;}    // Угол поворота
}

public class Cruiser : ISpaceship
{
    public int Speed => 50;
    public int FirePower => 100;
    public double x { get; set; }
    public double y { get; set; }
    public double angle { get; set; }

    public void MoveForward()
    {
        x += Math.Cos(angle) * Speed;
        y += Math.Sin(angle) * Speed;
    }

    public void Rotate(int angle)
    {
        this.angle += angle;
    }

    public void Fire()
    {

    }
}

public class Fighter : ISpaceship
{
    public int Speed => 75;
    public int FirePower => 200;
    public double x { get; set; }
    public double y { get; set; }
    public double angle { get; set; }

    public void MoveForward()
    {
        x += Math.Cos(angle) * Speed;
        y += Math.Sin(angle) * Speed;
    }

    public void Rotate(int angle)
    {
        this.angle += angle;
    }

    public void Fire()
    {

    }
}