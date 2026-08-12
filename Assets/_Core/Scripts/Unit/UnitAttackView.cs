using System;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitAttack : IUpdatable
{
    public event Action OnAttackStarted;
    public event Action OnAttackFinished;

    public void Enable();
    public void Disable();

    public void RegisterNewPossibleTarget(Collider possibleTarget);
    public void RemovePossibleTarget(Collider noLongerPossibleTarget);
}

public class UnitAttackView : MonoBehaviour
{
    private IUnitAttack _attackModel;

    public void Init(IUnitAttack attackModel) => _attackModel = attackModel;

    private void OnTriggerEnter(Collider other) => _attackModel.RegisterNewPossibleTarget(other);
    private void OnTriggerExit(Collider other) => _attackModel.RemovePossibleTarget(other);
}

public class UnitAttacker : IUnitAttack
{
    private UnitConfig _unitConfig;
    private Health _health;

    private AttackState _state;
    private float _timer;
    private float _updateFrequencyTimer;
    private List<IDamageable> _foundTargets = new List<IDamageable>(8);
    private HashSet<Collider> _possibleTargets = new HashSet<Collider>(8);

    private bool _isDisabled;

    private const float UpdateFrequency = 0.1f;

    public event Action OnAttackStarted;
    public event Action OnAttackFinished;

    public UnitAttacker(UnitConfig unitConfig, Health health)
    {
        _unitConfig = unitConfig;
        _health = health;
    }

    public void Enable()
    {
        _isDisabled = false;
        _state = AttackState.Ready;
    }

    public void Disable()
    {
        _isDisabled = true;
        _state = AttackState.Ready;
    }

    public void RegisterNewPossibleTarget(Collider possibleTarget) => _possibleTargets.Add(possibleTarget);
    public void RemovePossibleTarget(Collider noLongerPossibleTarget) => _possibleTargets.Remove(noLongerPossibleTarget);

    public void UpdateLogic(float deltaTime)
    {
        if (_health.IsDead || _isDisabled) return;

        _updateFrequencyTimer -= deltaTime;
        if (_updateFrequencyTimer < 0)
            _updateFrequencyTimer = UpdateFrequency;

        switch (_state)
        {
            case AttackState.Ready:
                ExtractValidTargetsToList();
                if (_foundTargets.Count > 0)
                    StartWindUp();
                break;
            case AttackState.WindUp:
                _timer -= deltaTime;
                if (_timer <= 0)
                    PerformAttack();
                break;
            case AttackState.CoolDown:
                _timer -= deltaTime;
                if (_timer <= 0)
                    _state = AttackState.Ready;
                break;
        }
    }

    private void FinishAttack()
    {
        _timer = _unitConfig.AttackCooldown;
        _state = AttackState.CoolDown;
        OnAttackFinished?.Invoke();
    }

    private void PerformAttack()
    {
        foreach (var target in _foundTargets)
        {
            if (_health.IsDead == true)
                return;

            target.TakeDamage(_unitConfig.Damage);

            if (target.ReturnsDamage)
                _health.TakeDamage(_unitConfig.Damage);
        }
        FinishAttack();
    }

    private void StartWindUp()
    {
        _timer = _unitConfig.AttackWindUpDelay;
        _state = AttackState.WindUp;
        OnAttackStarted?.Invoke();
    }

    private void ExtractValidTargetsToList()
    {
        // How many reflected hits can this unit still survive?
        int maxTargets = Mathf.Max(1, Mathf.CeilToInt(_health.CurrentHealth / (float)_unitConfig.Damage));
        _foundTargets.Clear();
        foreach (var collider in _possibleTargets)
        {
            if (collider == null)
                continue;
            
            if (collider.TryGetComponent(out IDamageable damageable) && 
                damageable.IsDead && damageable.BattleSide != _unitConfig.BattleSide)
            {
                _foundTargets.Add(damageable);
                if (_foundTargets.Count >= maxTargets)
                    return;
            }
        }

        _possibleTargets.Clear();
    }

    private enum AttackState
    {
        Ready,
        WindUp,
        CoolDown,
    }
}