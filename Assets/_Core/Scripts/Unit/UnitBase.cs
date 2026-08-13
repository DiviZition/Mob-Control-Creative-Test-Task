using System;

public interface IUnitModel : IUpdatable, IDisposable
{
    public IHealth Health { get; }
    public IUnitMovement Movement { get; }
    public IUnitAttack Attack { get; }
    public UnitConfig Config { get; }

    public void ReturnUnitToPool();
}

public class UnitBase : IUnitModel
{
    public IUnitMovement Movement { get; private set; }
    public IUnitAttack Attack { get; }
    public UnitConfig Config {  get; private set; }
    public IHealth Health => _health;

    private UnitSpawner _spawner;
    private Health _health;
    private bool IsDisabled;

    public UnitBase(UnitSpawner spawner, Health health, IUnitMovement movement, IUnitAttack attack, UnitConfig config)
    {
        _spawner = spawner;
        _health = health;
        Config = config;
        Movement = movement;
        Attack = attack;

        _health.OnDead += ReturnUnitToPool;

        Attack.OnAttackStarted += Movement.Unlock;
        //Call for the animation when OnAttackStarted fired
        Attack.OnAttackFinished += Movement.Lock;
    }

    public void UpdateLogic(float deltaTime)
    {
        if (IsDisabled == true || _health.IsDead == true)
            return;

        Attack.UpdateLogic(deltaTime);
        Movement.MoveUnitInDirection(deltaTime);
    }

    public void EnableUnit()
    {
        _health.ResetCurrentHealth();

        Movement.ResetToInitialState();
        Movement.ForceRemoveAllLockers();
        Movement.Lock();

        Attack.Enable();

        IsDisabled = false;
    }

    public void DisableUnit()
    {
        IsDisabled = true;

        Movement.Unlock();
        Attack.Disable();
    }

    public void ReturnUnitToPool()
    {
        DisableUnit();
        _spawner.DeactivateUnit(this);
    }

    public void Dispose()
    {
        _health.OnDead -= ReturnUnitToPool;

        Attack.OnAttackStarted -= Movement.Unlock;
        Attack.OnAttackFinished -= Movement.Lock;
    }
}

public enum UnitBattleSide
{
    Player,
    Enemy,
}
