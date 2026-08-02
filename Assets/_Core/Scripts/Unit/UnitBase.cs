using R3;
using System;
using UnityEngine;

[SelectionBase]
public class UnitBase : Damageable, IPoolable, IDamageable
{
    [field: SerializeField] public Transform Transform { get; private set; }
    [field: SerializeField] public UnitMovement Movement { get; private set; }
    [field: SerializeField] public UnitView View { get; private set; }

    public MultiplyingGate IgnoreGate { get; private set; }

    [SerializeField] private UnitDamageDealer _damageDealer;
    [SerializeField] private Collider _collider;

    private UnitSpawner _spawner;

    public void ActivatePoolable()
    {
        RestoreHealth();
        Movement.ResetAgent();
        Movement.Enable();
        this.gameObject.SetActive(true);
        View.PlayAnimation(AnimationType.Run);
        _collider.enabled = true;
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

    protected override void PerformDeath()
    {
        base.PerformDeath();
        Movement.Disable();
        _collider.enabled = false;
        Action onFXEnd = () => _spawner.DeactivateUnit(this);
        View.PlayDeadAnimation(onFXEnd);
    }
}

public enum UnitBattleSide
{
    Player,
    Enemy,
}
