using System;
using UnityEngine;

public interface IDamageable
{
    public Transform Transform { get; }
    public UnitBattleSide BattleSide { get; }
    public bool ReturnsDamage { get; }
    public bool IsDead { get; }

    public void TakeDamage(int damage);
}
