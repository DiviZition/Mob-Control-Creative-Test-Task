using System;
using UnityEngine;

public class UnitHealth : MonoBehaviour, IDamageable
{
    [field: SerializeField] public UnitBattleSide BattleSide {  get; private set; }
    [field: SerializeField] public bool ReturnsDamage { get; private set; }

    [field: SerializeField] public int MaxHealth {  get; private set; }
    public int CurrentHealth { get; private set; }


    public event Action OnDead;

    public void TakeDamage(int damage)
    {
        if (CurrentHealth - damage <= 0)
        {
            CurrentHealth = 0;
            OnDead?.Invoke();
            return;
        }

        CurrentHealth -= damage;
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }
}
