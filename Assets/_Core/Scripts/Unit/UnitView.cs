using System;
using UnityEngine;

public interface IUnitView : IDamageable
{
    public void SetViewEnabled(bool isEnabled);
    public void ForceDisableUnit();

    public void Movement_SetSpeed(float newSpeed);
    public void Movement_SetDirection(Quaternion newDirection);
    public void Movement_SetPosition(Vector3 newPosition);
    public void Movement_Lock();
    public void Movement_UnLock();

    public IHealth Health_GetHealth();
}

[RequireComponent(typeof(UnitAnimations))]
[RequireComponent(typeof(UnitAttackView))]
[RequireComponent(typeof(UnitMovementView))]
public class UnitView : MonoBehaviour, IUnitView
{
    [field: SerializeField] public UnitAnimations Animations { get; private set; }
    [field: SerializeField] public UnitAttackView AttackView { get; private set; }
    [field: SerializeField] public UnitMovementView MovementView { get; private set; }
    [field: SerializeField] public Transform Transform { get; private set; }

    private IUnitModel _unitModel;
    public UnitBattleSide BattleSide => _unitModel.Config.BattleSide;
    public bool ReturnsDamage => _unitModel.Config.ReturnsDamage;
    public bool IsDead => _unitModel.Health.IsDead;


    private void OnValidate()
    {
        Animations = Animations ?? GetComponent<UnitAnimations>();
        AttackView = AttackView ?? GetComponent<UnitAttackView>();
        MovementView = MovementView ?? GetComponent<UnitMovementView>();
        Transform = Transform ?? GetComponent<Transform>();
    }

    public void Init(IUnitModel unitModel)
    {
        MovementView.Init(unitModel.Movement, Transform);
        AttackView.Init(unitModel.Attack);
        Animations.Init(unitModel);
    }

    void IDamageable.TakeDamage(int damage) => _unitModel.Health.TakeDamage(damage);
    public void SetViewEnabled(bool isEnabled) => gameObject.SetActive(isEnabled);

    public void ForceDisableUnit() => _unitModel.ReturnUnitToPool();

    public void Movement_Lock() => _unitModel.Movement.Lock();
    public void Movement_UnLock() => _unitModel.Movement.Unlock();
    public void Movement_SetPosition(Vector3 newPosition) => _unitModel.Movement.TeleportToPosition(newPosition);
    public void Movement_SetSpeed(float newSpeed) => _unitModel.Movement.SetNewMoveSpeed(newSpeed);
    public void Movement_SetDirection(Quaternion newDirection) => _unitModel.Movement.SetDirection(newDirection);

    public IHealth Health_GetHealth() => _unitModel.Health;
}
