using System;
using UnityEngine;

public class DamageableView : MonoBehaviour, IDamageable
{
    [field: SerializeField] public UnitBattleSide BattleSide { get; private set; }
    [field: SerializeField] public bool ReturnsDamage { get; private set; }
    [field: SerializeField] public Transform Transform { get; private set; }

    public bool IsDead { get; private set; }

    public event Action<int> OnDamageTaken;

    private void OnValidate() => Transform ??= transform;
    void IDamageable.TakeDamage(int damage) => OnDamageTaken?.Invoke(damage);
}
