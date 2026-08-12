using System;

public interface ICanonShooter : IUpdatable
{
    public event Action OnCanonShoot;
    public void ShootWithUnit();
}
