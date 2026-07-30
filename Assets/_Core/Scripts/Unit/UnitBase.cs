using R3;
using System;
using UnityEngine;

public class UnitBase : MonoBehaviour, IPoolable
{
    [field: SerializeField] public Transform Transform { get; private set; }
    [field: SerializeField] public UnitMovement Movement { get; private set; }
    [field: SerializeField] public UnitBaseFX FX { get; private set; }

    public UnitBattleSide UnitBattleSide => _health.BattleSide;
    public MultiplyingGate IgnoreGate { get; private set; }

    [SerializeField] private UnitHealth _health;
    [SerializeField] private UnitDamageDealer _damageDealer;

    private UnitSpawner _spawner;

    public void ActivatePoolable()
    {
        Movement.ResetAgent();
        Movement.Enable();
        FX.ResetToNormal();
        this.gameObject.SetActive(true);
        FX.PlaySpawnVFX();
    }

    public void DeactivatePoolable()
    {
        Movement.Disable();
        this.gameObject.SetActive(false);
    }

    public void SetGateToIgnore(MultiplyingGate ignoreGate) => IgnoreGate = ignoreGate;
    //TODO: Consider getting spawner via injection, or any way to remove this interaction from UnitBase. 
    public void SetUnitSpawner(UnitSpawner spawner) => _spawner = spawner;

    private void OnDead()
    {
        Movement.Disable();
        Action onFXEnd = () => _spawner.DeactivateUnit(this);
        FX.PlayDeadFx(onFXEnd);
    }

    private void OnEnable()
    {
        _health.OnDead += OnDead;
        _health.ResetHealth();
    }

    private void OnDisable()
    {
        _health.OnDead -= OnDead;
    }
}

public enum UnitBattleSide
{
    Player,
    Enemy,
}

internal interface IDamageable
{
    public int MaxHealth { get; }
    public int CurrentHealth { get; }
    public UnitBattleSide BattleSide { get; }
    public event Action OnDead;

    public void TakeDamage(int damage);
}
