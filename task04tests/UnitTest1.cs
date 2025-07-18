public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }

    [Fact]
    public void Fighter_ShouldHaveCorrectStats()
    {
        ISpaceship fighter = new Fighter();
        Assert.Equal(75, fighter.Speed);
        Assert.Equal(200, fighter.FirePower);
    }

    [Fact]
    public void Cruiser_MoveForward_Test()
    {
        ISpaceship cruiser = new Cruiser();
        cruiser.MoveForward();
        Assert.Equal(50, cruiser.x);
    }

    [Fact]
    public void Fighter_Rotate_Test()
    {
        ISpaceship fighter= new Fighter();
        fighter.Rotate(5);
        Assert.Equal(5, fighter.angle);
    }
}