using UnityEngine;
using System;
using TMPro;
using MoreMountains.Feedbacks;

public interface IMultiplyingGateView
{
    public int InitialMultiplyer { get; }
    public event Action<int, int, UnitBattleSide, Vector3, Quaternion> OnUnitEntered;

    public void OnGateUpgrade(int newMultiplyingValue);
}

public class MultiplyingGateView : MonoBehaviour, IMultiplyingGateView
{
    [field: SerializeField] public int InitialMultiplyer { get; private set; }
    [field: SerializeField] public TMP_Text GatesXValueText { get; private set; }
    [field: SerializeField] public MMF_Player GatesUpgradeFeedback { get; private set; }

    public event Action<int, int, UnitBattleSide, Vector3, Quaternion> OnUnitEntered;
    public int GateViewKey { get; private set; }

    public void SetKey(int key) => GateViewKey = key;
    public void OnGateUpgrade(int newMultiplyingValue)
    {
        GatesUpgradeFeedback.RestoreInitialValues();
        GatesUpgradeFeedback.ResetFeedbacks();
        GatesUpgradeFeedback.PlayFeedbacks();

        GatesXValueText.text = $"X{newMultiplyingValue.ToString()}";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UnitView unitView))
            OnUnitEntered?.Invoke(GateViewKey, unitView.ID, unitView.BattleSide, unitView.Transform.position, unitView.Transform.localRotation);
    }
}
