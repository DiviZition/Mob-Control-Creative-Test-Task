using PrimeTween;
using System;
using UnityEngine;

public class BossUnitAttacker : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _attackCoolDown;
    [SerializeField] private LayerMask _unitToHitMask;
    [SerializeField] private UnitBase _unitBase;

    private bool _performingAttack;

    private void OnTriggerStay(Collider other)
    {
        if (_performingAttack == false && other.TryGetComponent(out PlayerTower damageable))
        {
            _performingAttack = true;
            _unitBase.Movement.Disable();
            Action onAnimationFinished = () => PerformAttack();
            _unitBase.View.PlayAnimation(AnimationType.Attack, onAnimationFinished);
        }
    }

    private void PerformAttack()
    {
        var colliders = Physics.OverlapSphere(_unitBase.Transform.position, _attackRadius, _unitToHitMask);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out IDamageable damageable) && damageable.BattleSide != _unitBase.BattleSide)
            {
                damageable.TakeDamage(_damage);
            }
        }

        Tween.Delay(_attackCoolDown, () =>
        {
            _performingAttack = false;

            _unitBase.View.PlayAnimation(AnimationType.Run);
            _unitBase.Movement.Enable();
        });
    }

    private void OnDrawGizmos()
    {
        if (_unitBase != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_unitBase.Transform.position, _attackRadius);
        }
    }
}
