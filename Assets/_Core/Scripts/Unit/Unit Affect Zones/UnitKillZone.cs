using UnityEngine;

public class UnitKillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(999);
    }
}
