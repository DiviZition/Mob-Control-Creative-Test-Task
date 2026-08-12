public interface ICanonModel : IUpdatable, ILockable
{
    public ICanonMovement Movement { get; }
    public ICanonShooter Shooter { get; }
    public CanonSettingsConfig Config { get; }
}
