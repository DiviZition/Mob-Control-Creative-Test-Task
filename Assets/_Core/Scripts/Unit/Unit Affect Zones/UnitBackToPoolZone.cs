using UnityEngine;

public class UnitBackToPoolZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IPoolable poolable))
            poolable.DeactivatePoolable();
    }
}
