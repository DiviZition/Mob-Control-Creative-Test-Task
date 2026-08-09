using UnityEngine;

public class UnitDamageDealer : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _attackCoolDown;
    [SerializeField] private UnitBase _unitBase;

    private float _nextAttackAvailable;

    private void OnTriggerStay(Collider other)
    {
        if (_nextAttackAvailable > Time.time)
            return;

        if (other.TryGetComponent(out IDamageable damageable) && damageable.BattleSide != _unitBase.BattleSide)
        {
            damageable.TakeDamage(_damage);

            //Debug.Log($"{this.gameObject.name} killed {other.gameObject.name}");
            if (damageable.ReturnsDamage == true)
                _unitBase.TakeDamage(_damage);

            _nextAttackAvailable = Time.time + _attackCoolDown;
        }
    }
}