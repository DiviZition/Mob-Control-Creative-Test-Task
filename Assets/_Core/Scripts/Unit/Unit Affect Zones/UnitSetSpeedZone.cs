using UnityEngine;

public class UnitSetSpeedZone : MonoBehaviour
{
    [SerializeField] private float _newSpeed;
    [SerializeField] private UnitBattleSide _whoToAffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IUnitView unit) && unit.BattleSide == _whoToAffect)
            unit.CallChangeMoveSpeed(_newSpeed);
    }
}
