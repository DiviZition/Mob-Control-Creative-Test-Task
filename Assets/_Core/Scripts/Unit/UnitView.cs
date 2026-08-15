using System;
using UnityEngine;

public interface IUnitView : IDamageable
{
    public int ID { get; }

    public void Enable();
    public void Disable();
    public void ForceDisableUnit();

    public void Movement_SetSpeed(float newSpeed);
    public void Movement_SetDirection(Quaternion newDirection);
    public void Movement_SetPosition(Vector3 newPosition);
    public void Movement_Lock();
    public void Movement_UnLock();

    public Health Health_GetHealth();
}

[SelectionBase]
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

    public int ID => _unitModel.ID;

    private void OnValidate()
    {
        if (Animations == null) Animations = GetComponent<UnitAnimations>();
        if (AttackView == null) AttackView = GetComponent<UnitAttackView>();
        if (MovementView == null) MovementView = GetComponent<UnitMovementView>();
        if (Transform == null) Transform = transform;
    }

    public void Init(IUnitModel unitModel)
    {
        _unitModel = unitModel;
        MovementView.Init(_unitModel.Movement, Transform);
        AttackView.Init(_unitModel.Attack);
        Animations.Init(_unitModel.Config.PlayAttackAnimation, _unitModel.Attack);

        _unitModel.OnEnabled += Enable;
        _unitModel.OnDied += OnUnitDied;
    }


    private void OnDestroy()
    {
        if (_unitModel == null) return;

        _unitModel.OnEnabled -= Enable;
        _unitModel.OnDied -= OnUnitDied;
    }

    private void OnUnitDied()
    {
        Action unpoolModelAndDisableView = () =>
        {
            _unitModel.ReturnUnitToPool();
            Disable();
        };

        Animations.PlayDeadAnimation(unpoolModelAndDisableView);
    }

    public void Enable()
    {
        MovementView.Enable();
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        MovementView.Disable();
        gameObject.SetActive(false);
    }

    void IDamageable.TakeDamage(int damage) => _unitModel.Health.TakeDamage(damage);

    void IUnitView.ForceDisableUnit() => _unitModel.ReturnUnitToPool();

    void IUnitView.Movement_Lock() => _unitModel.Movement.Lock();
    void IUnitView.Movement_UnLock() => _unitModel.Movement.Unlock();
    void IUnitView.Movement_SetPosition(Vector3 newPosition) => _unitModel.Movement.TeleportToPosition(newPosition);
    void IUnitView.Movement_SetSpeed(float newSpeed) => _unitModel.Movement.SetNewMoveSpeed(newSpeed);
    void IUnitView.Movement_SetDirection(Quaternion newDirection) => _unitModel.Movement.SetDirection(newDirection);

    Health IUnitView.Health_GetHealth() => _unitModel.Health;
}
