using System;
using UnityEngine;

public interface IDamageable
{
    public int MaxHealth { get; }
    public int CurrentHealth { get; }

    public Transform Transform { get; }
    public bool ReturnsDamage { get; }
    public UnitBattleSide BattleSide { get; }

    public event Action OnDead;

    public void TakeDamage(int damage);
}
