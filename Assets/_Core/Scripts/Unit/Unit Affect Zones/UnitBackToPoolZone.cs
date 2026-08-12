using UnityEngine;

public class UnitBackToPoolZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IUnitView unit))
            unit.CallForceDissapear();
    }
}
