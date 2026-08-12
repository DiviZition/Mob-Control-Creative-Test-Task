using System;
using UnityEngine;
using UnityEngine.AI;

public class UnitView : MonoBehaviour, IUnitView
{
    [field: SerializeField] public UnitVisual Visual { get; private set; }
    [field: SerializeField] public UnitAttackView AttackView { get; private set; }
    [field: SerializeField] public UnitMovementView MovementView { get; private set; }
    [field: SerializeField] public Collider Collider { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public Transform Transform { get; private set; }

    public IUnitModel UnitModel { get; private set; }
    public UnitBattleSide BattleSide => UnitModel.Config.BattleSide;
    public bool ReturnsDamage => UnitModel.Config.ReturnsDamage;

    public bool IsDead => UnitModel.Health.IsDead;

    public void Init(IUnitModel unitModel)
    {
        MovementView.Init(unitModel.Movement);
        AttackView.Init(unitModel.Attack);
    }

    void IDamageable.TakeDamage(int damage) => UnitModel.Health.TakeDamage(damage);

    public void SetViewEnabled(bool isEnabled) => gameObject.SetActive(isEnabled);
    public void SetCollisionEnabled(bool isEnabled) => Collider.enabled = isEnabled;
}

public interface IUnitView : IDamageable
{
    public IUnitModel UnitModel { get; }
    public void SetViewEnabled(bool isEnabled);
}

public interface IUnitModel : IUpdatable, IDisposable
{
    public IHealth Health { get; }
    public IUnitMovement Movement { get; }
    public IUnitAttack Attack { get; }
    public UnitConfig Config { get; }
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

        _health.OnDead += PerformDeath;

        Attack.OnAttackStarted += Movement.Disable;
        //Call for the animation when OnAttackStarted fired
        Attack.OnAttackFinished += Movement.Enable;
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
        Movement.Enable();

        Attack.Enable();

        IsDisabled = false;
    }

    public void DisableUnit()
    {
        IsDisabled = true;

        Movement.Disable();
        Attack.Disable();
    }

    private void PerformDeath()
    {
        DisableUnit();
        _spawner.DeactivateUnit(this);
    }

    public void Dispose()
    {
        _health.OnDead -= PerformDeath;

        Attack.OnAttackStarted -= Movement.Disable;
        Attack.OnAttackFinished -= Movement.Enable;
    }
}

public enum UnitBattleSide
{
    Player,
    Enemy,
}
