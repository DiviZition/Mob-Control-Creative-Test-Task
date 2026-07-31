using R3;
using System;
using UnityEngine;

[SelectionBase]
public class UnitBase : MonoBehaviour, IPoolable, IDamageable
{
    [field: SerializeField] public Transform Transform { get; private set; }
    [field: SerializeField] public UnitMovement Movement { get; private set; }
    [field: SerializeField] public UnitBaseFX FX { get; private set; }
    [field: SerializeField] public UnitBattleSide BattleSide { get; private set; }

    public MultiplyingGate IgnoreGate { get; private set; }

    [SerializeField] private UnitDamageDealer _damageDealer;
    [SerializeField] private Collider _collider;

    [Header("Health")]
    [field: SerializeField] public bool ReturnsDamage { get; private set; }
    [field: SerializeField] public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    public event Action OnDead;

    private UnitSpawner _spawner;

    public void ActivatePoolable()
    {
        ResetHealth();
        Movement.ResetAgent();
        Movement.Enable();
        FX.ResetToNormal();
        this.gameObject.SetActive(true);
        _collider.enabled = true;
        FX.PlaySpawnVFX();
    }

    public void DeactivatePoolable()
    {
        Movement.Disable();
        SetGateToIgnore(null);
        this.gameObject.SetActive(false);
    }

    public void SetGateToIgnore(MultiplyingGate ignoreGate) => IgnoreGate = ignoreGate;
    //TODO: Consider getting spawner via injection, or any way to remove this interaction from UnitBase. 
    public void SetUnitSpawner(UnitSpawner spawner) => _spawner = spawner;

    private void PerformDeath()
    {
        IsDead = true;
        Movement.Disable();
        _collider.enabled = false;
        Action onFXEnd = () => _spawner.DeactivateUnit(this);
        FX.PlayDeadFx(onFXEnd);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead == true)
            return;

        if (CurrentHealth - damage <= 0)
        {
            CurrentHealth = 0;
            PerformDeath();
            OnDead?.Invoke();
            return;
        }

        CurrentHealth -= damage;
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
        IsDead = false;
    }
}

public enum UnitBattleSide
{
    Player,
    Enemy,
}
