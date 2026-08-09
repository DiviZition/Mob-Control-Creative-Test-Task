using UnityEngine;

public class UnitSetSpeedZone : MonoBehaviour
{
    [SerializeField] private float _newSpeed;
    [SerializeField] private UnitBattleSide _whoToAffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UnitBase unit) && unit.BattleSide == _whoToAffect)
            unit.Movement.SetNewMoveSpeed(_newSpeed);
    }
}
