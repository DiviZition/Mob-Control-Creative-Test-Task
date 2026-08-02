using UnityEngine;
using System;
using TMPro;
using MoreMountains.Feedbacks;

public class MultiplyingGate : MonoBehaviour
{
    [SerializeField] private int _initialMultiplyingValue;
    [SerializeField] private float _spawnPositionRandomOffset;
    [SerializeField] private UnitBattleSide _whoToMultiply;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private TMP_Text _gatesXValueText;
    [SerializeField] private MMF_Player _gatesUpgradeFeedback;

    public int CurrentMultiplyingValue => _initialMultiplyingValue + _additionalMultiplyingValue;
    private int _additionalMultiplyingValue;

    private void Start() => UpdateText();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UnitBase unit) && unit.BattleSide == _whoToMultiply && unit.IgnoreGate != this)
        {
            unit.SetGateToIgnore(this);
            for (int i = 0; i < CurrentMultiplyingValue - 1; i++)
            {
                Vector3 spawnPosition = unit.Transform.position;
                spawnPosition += (UnityEngine.Random.insideUnitSphere * _spawnPositionRandomOffset).ResetY();
                
                Action<UnitBase> actionBeforeActivate = (unit) => unit.SetGateToIgnore(this);
                _unitSpawner.SpawnUnit(spawnPosition, unit.Transform.localRotation, actionBeforeActivate);
            }
        }
    }

    public void IncreaseMultiplyingValue(int additionalValue)
    {
        _additionalMultiplyingValue += additionalValue;
        _gatesUpgradeFeedback.PlayFeedbacks();
        UpdateText();
    }

    private void UpdateText() => _gatesXValueText.text = $"X{CurrentMultiplyingValue}";
}