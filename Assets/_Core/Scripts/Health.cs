using System;
using UnityEngine;

public class Health
{
    public int MaxHealth { get; private set; }
    public int CurrentHealth {  get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    public event Action<int> OnHealthChanged;
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

        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
        if (CurrentHealth <= 0)
            OnDead?.Invoke();

        OnHealthChanged?.Invoke(-damage);
    }

    public void ResetCurrentHealth() => SetCurrentHealth(MaxHealth);
    public void SetCurrentHealth(int currentHealth)
    {
        OnHealthChanged?.Invoke(currentHealth - CurrentHealth);
        CurrentHealth = currentHealth;
    }
}
