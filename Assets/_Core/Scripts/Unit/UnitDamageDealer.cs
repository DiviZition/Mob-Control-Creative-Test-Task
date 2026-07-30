using UnityEngine;

public class UnitDamageDealer : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _attackCoolDown;
    [SerializeField] private UnitBattleSide _whoToAttack;
    [SerializeField] private UnitHealth _unitHealth;

    private float _nextAttackAvailable;

    private void OnTriggerStay(Collider other)
    {
        if (_nextAttackAvailable > Time.time)
            return;

        if (other.TryGetComponent(out IDamageable damageable) && damageable.BattleSide == _whoToAttack)
        {
            damageable.TakeDamage(_damage, _unitHealth);
            _nextAttackAvailable = Time.time + _attackCoolDown;
        }
    }
}