using System;

public interface IUnitModel : IUpdatable, IDisposable
{
    public Health Health { get; }
    public IUnitMovement Movement { get; }
    public IUnitAttack Attack { get; }
    public UnitConfig Config { get; }
    public int ID { get; }

    public event Action OnEnabled;
    public event Action OnDied;

    public void EnableUnit();
    public void DisableUnitLogic();
    public void ReturnUnitToPool();
}

public class UnitBase : IUnitModel
{
    public IUnitMovement Movement { get; private set; }
    public IUnitAttack Attack { get; }
    public UnitConfig Config {  get; private set; }
    public Health Health => _health;

    public int ID { get; private set; }

    private UnitSpawner _spawner;
    private Health _health;
    private bool IsDisabled;

    public event Action OnEnabled;
    public event Action OnDied;

    public UnitBase(UnitSpawner spawner, Health health, IUnitMovement movement, IUnitAttack attack, UnitConfig config, int id)
    {
        _spawner = spawner;
        _health = health;
        Config = config;
        Movement = movement;
        Attack = attack;
        ID = id;

        _health.OnDead += OnDead;

        Attack.OnAttackStarted += Movement.Unlock;
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

        Attack.Enable();

        OnEnabled?.Invoke();
        IsDisabled = false;
    }

    public void DisableUnitLogic()
    {
        IsDisabled = true;

        Movement.Lock();
        Attack.Disable();
    }

    private void OnDead()
    {
        DisableUnitLogic();
        OnDied?.Invoke();
    }

    public void ReturnUnitToPool()
    {
        DisableUnitLogic();
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
