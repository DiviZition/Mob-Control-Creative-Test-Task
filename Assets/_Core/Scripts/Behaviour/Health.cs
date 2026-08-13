using System;
using UnityEngine;

public interface IHealth
{
    public int MaxHealth { get; }
    public int CurrentHealth { get; }
    public bool IsDead { get; }

    public event Action<int, int> OnHealthChanged;
    public event Action OnDead;

    public void TakeDamage(int damage);
}

public class Health : IHealth
{
    public int MaxHealth { get; private set; }
    public int CurrentHealth {  get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDead;

    public Health(int maxHP)
    {
        MaxHealth = maxHP;
        CurrentHealth = MaxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        if (IsDead)
            return;

        //int currentHealthSnapshot = CurrentHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
        if (CurrentHealth <= 0)
            OnDead?.Invoke();

        OnHealthChanged?.Invoke(MaxHealth, CurrentHealth);
    }

    public void ResetCurrentHealth() => SetCurrentHealth(MaxHealth);
    public void SetCurrentHealth(int currentHealth)
    {
        CurrentHealth = currentHealth;
        OnHealthChanged?.Invoke(MaxHealth, CurrentHealth);
    }
}
