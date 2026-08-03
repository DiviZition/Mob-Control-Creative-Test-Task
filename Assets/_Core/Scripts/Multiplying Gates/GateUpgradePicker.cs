using MoreMountains.Feedbacks;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GateUpgradePicker : MonoBehaviour
{
    [SerializeField] private GatesUpgradeHandler _gatesUpgrader;
    [SerializeField] private Collider _collider;
    [SerializeField] private int _valueToApply;
    [SerializeField] private bool _isMultiplying;
    [SerializeField] private MMF_Player _pickUpEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UnitBase unit) && unit.BattleSide == UnitBattleSide.Player)
        {
            _gatesUpgrader.UpgradeAllGatesX(_valueToApply, _isMultiplying);
            _collider.enabled = false;
            _pickUpEffect.PlayFeedbacks();
        }
    }
}
