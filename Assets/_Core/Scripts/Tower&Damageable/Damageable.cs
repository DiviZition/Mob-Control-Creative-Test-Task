using System;
using UnityEngine;

public class Damageable : MonoBehaviour, IDamageable
{
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public UnitBattleSide BattleSide { get; private set; }
    [field: SerializeField] public bool ReturnsDamage { get; private set; }

    public int CurrentHealth {  get; private set; }
    public bool IsDead { get; private set; }

    public event Action OnTakeDamage;
    public event Action OnHealthRestored;
    public event Action OnDead;

    public virtual void TakeDamage(int damage)
    {
        if (IsDead)
            return;

        if (CurrentHealth - damage <= 0)
        {
            CurrentHealth = 0;
            PerformDeath();
        }
        else
        {
            CurrentHealth -= damage;
        }

        OnTakeDamage?.Invoke();
    }

    protected virtual void PerformDeath() => OnDead?.Invoke();

    protected virtual void RestoreHealth()
    {
        CurrentHealth = MaxHealth;
        IsDead = false;
        OnHealthRestored?.Invoke();
    }
}
