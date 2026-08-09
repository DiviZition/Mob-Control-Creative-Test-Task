using System;
using UnityEngine;
using UnityEngine.AI;

public class UnitView : MonoBehaviour, IUnitView
{
    [field: SerializeField] public UnitVisual Visual { get; private set; }
    [field: SerializeField] public Collider Collider { get; private set; }
    [field: SerializeField] public DamageableView Damageable { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public Transform Transform { get; private set; }
    [field: SerializeField] public UnitBattleSide BattleSide { get; private set; }

    public event Action<int> OnDamageTaken
    {
        add { Damageable.OnDamageTaken += value; }
        remove {  Damageable.OnDamageTaken -= value;}
    }

    void IUnitView.PlayAnimation(AnimationType type) => Visual.PlayAnimation(type);
    void IUnitView.PlayDeadAnimation(Action onAnimationEnd) => Visual.PlayDeadAnimation(onAnimationEnd);
    void IUnitView.DisableCollision() => Collider.enabled = false;
    void IUnitView.EnableCollision() => Collider.enabled = true;
    void IUnitView.DisableView() => gameObject.SetActive(false);
    void IUnitView.EnableView() => gameObject.SetActive(true);

    public void GetPositionAndRotation(out Vector3 position, out Quaternion rotation)
    {
        position = Transform.position;
        rotation = Transform.localRotation;
    }
}

public interface IUnitView
{
    public UnitBattleSide BattleSide { get; }

    public event Action<int> OnDamageTaken;

    public void GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
    public void PlayAnimation(AnimationType type);
    public void PlayDeadAnimation(Action onAnimationEnd);
    public void DisableCollision();
    public void EnableCollision();
    public void DisableView();
    public void EnableView();
}

public class UnitBase : IUpdatable, IDisposable
{
    public IUnitMovement Movement { get; private set; }
    public IUnitView View { get; private set; }
    public Health Health { get; private set; }

    private UnitSpawner _spawner;

    public UnitBase(IUnitView view, UnitSpawner spawner, Health health, IUnitMovement movement)
    {
        View = view;
        _spawner = spawner;
        Health = health;
        Movement = movement;

        View.OnDamageTaken += Health.TakeDamage;
        Health.OnDead += PerformDeath;
    }

    public void UpdateLogic(float deltaTime) => Movement.MoveUnitInDirection(deltaTime);

    public void EnableUnit()
    {
        Health.ResetCurrentHealth();

        Movement.ResetToInitialState();
        Movement.Enable();
        
        View.EnableCollision();
        View.EnableView();
        View.PlayAnimation(AnimationType.Run);
    }

    public void DisableUnit()
    {
        View.DisableView();
        Movement.Disable();
    }

    public void PerformDeath()
    {
        View.DisableCollision();
        Movement.Disable();
        Action onFXEnd = () => _spawner.DeactivateUnit(this);
        View.PlayDeadAnimation(onFXEnd);
    }

    public void Dispose()
    {
        View.OnDamageTaken -= Health.TakeDamage;
        Health.OnDead -= PerformDeath;
    }
}

public enum UnitBattleSide
{
    Player,
    Enemy,
}
