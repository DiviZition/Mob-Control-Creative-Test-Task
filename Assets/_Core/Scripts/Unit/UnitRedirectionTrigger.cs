using UnityEngine;

public class UnitRedirectionTrigger : MonoBehaviour
{
    [SerializeField] private Transform _directionReference;
    [SerializeField] private UnitBattleSide _whoToRedirect;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out UnitBase unit) && unit.UnitBattleSide == _whoToRedirect)
        {
            unit.Movement.RotateUnit(_directionReference.rotation);
        }
    }
}
