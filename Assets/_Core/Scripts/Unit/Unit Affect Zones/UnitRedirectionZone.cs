using UnityEngine;

public class UnitRedirectionTrigger : MonoBehaviour
{
    [SerializeField] private UnitBattleSide _whoToRedirect;
    [SerializeField] private Transform _directionReference;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UnitBase unit) && unit.BattleSide == _whoToRedirect)
        {
            unit.Movement.RotateUnit(_directionReference.localRotation);
        }
    }
}