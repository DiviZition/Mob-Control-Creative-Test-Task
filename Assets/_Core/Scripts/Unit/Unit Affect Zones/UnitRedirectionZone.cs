using UnityEngine;

public class UnitRedirectionTrigger : MonoBehaviour
{
    [SerializeField] private UnitBattleSide _whoToRedirect;
    [SerializeField] private Transform _directionReference;

    private Quaternion _rotationDirection;
    private bool _wasCached = false;

    private void OnValidate()
    {
        if (_directionReference == null)
            _directionReference = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IUnitView unit) && unit.BattleSide == _whoToRedirect)
        {
            if (_wasCached == false)
            {
                _wasCached = true;
                _rotationDirection = _directionReference.rotation;
            }

            unit.Movement_SetDirection(_rotationDirection);
        }
    }
}