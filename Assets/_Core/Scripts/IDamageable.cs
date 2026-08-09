using System;
using UnityEngine;

public interface IDamageable
{
    public Transform Transform { get; }
    public bool ReturnsDamage { get; }
    public UnitBattleSide BattleSide { get; }

    public event Action<int> OnDamageTaken;

    public void TakeDamage(int damage);
}

public interface IInputProvider
{
    float AxisHorizontal { get; }
    bool IsShooting { get; }
}

public interface IUpdatable
{
    void UpdateLogic(float deltaTime);
}