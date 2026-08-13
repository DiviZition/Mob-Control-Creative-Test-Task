using MoreMountains.Feedbacks;
using System;
using UnityEngine;

public interface IGateUpgradeView
{
    public int ValueToApply { get; }
    public bool IsMultiplying { get; }

    public event Action<int, bool> OnUpgradePicked;
}

public class GateUpgradeView : MonoBehaviour, IGateUpgradeView
{
    [SerializeField] private Collider _collider;
    [SerializeField] private MMF_Player _pickUpEffect;

    [field: SerializeField] public int ValueToApply { get; private set; }
    [field: SerializeField] public bool IsMultiplying { get; private set; }

    public event Action<int, bool> OnUpgradePicked;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IUnitView unit) && unit.BattleSide == UnitBattleSide.Player)
        {
            OnUpgradePicked?.Invoke(ValueToApply, IsMultiplying);
            _collider.enabled = false;
            _pickUpEffect.PlayFeedbacks();
        }
    }
}