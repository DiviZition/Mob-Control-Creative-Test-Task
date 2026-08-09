using UnityEngine;
using System;
using TMPro;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using System.Linq;

public interface IMultiplyingGateView
{
    public int InitialMultiplyer { get; }
    public event Action<IMultiplyingGateView, IUnitView> OnUnitEntered;

    public void OnGateUpgrade(int newMultiplyingValue);
}

public class MultiplyingGateView : MonoBehaviour, IMultiplyingGateView
{
    [field: SerializeField] public int InitialMultiplyer { get; private set; }
    [field: SerializeField] public TMP_Text GatesXValueText { get; private set; }
    [field: SerializeField] public MMF_Player GatesUpgradeFeedback { get; private set; }

    public event Action<IMultiplyingGateView, IUnitView> OnUnitEntered;

    public void OnGateUpgrade(int newMultiplyingValue)
    {
        GatesXValueText.text = newMultiplyingValue.ToString();
        GatesUpgradeFeedback.RestoreInitialValues();
        GatesUpgradeFeedback.ResetFeedbacks();
        GatesUpgradeFeedback.PlayFeedbacks();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UnitView unitView))
            OnUnitEntered?.Invoke(this, unitView);
    }
}

public class MultiplyingGatesViewsProvider : MonoBehaviour
{
    [field: SerializeField] public IMultiplyingGateView[] GatesViews { get; private set; }
    [field: SerializeField] public IGateUpgradeView[] GatesUpgrades { get; private set; }
}

public class MultiplyingGateSystem : IDisposable, IUpdatable
{
    private UnitSpawner _unitSpawner;
    private Dictionary<IMultiplyingGateView, GateData> _gates;
    private float _localTime;
    private List<IUnitView> _unitsToRemoveFromIgnore = new(32);

    private const float TimeToIgnoreUnit = 0.5f;
    private const float SpawnPositionRandomOffset = 0.1f;

    public MultiplyingGateSystem(IMultiplyingGateView[] gatesVies, UnitSpawner unitSpawner)
    {
        _gates = gatesVies.ToDictionary(view => view, view => new GateData(view.InitialMultiplyer));
        _unitSpawner = unitSpawner;
        foreach (var view in _gates)
            view.Key.OnUnitEntered += MultiplyUnits;
    }

    public void UpdateLogic(float deltaTime)
    {
        _localTime += deltaTime;

        foreach (var view in _gates)
        {
            //Updating units to ignore timers
            _unitsToRemoveFromIgnore.Clear();
            var gateData = view.Value;

            foreach (var unitToIgnore in gateData.UnitsToIgnore)
            {
                float removeTime = unitToIgnore.Value;
                if (removeTime <= _localTime)
                    _unitsToRemoveFromIgnore.Add(unitToIgnore.Key);
            }

            foreach (var unitToRemove in _unitsToRemoveFromIgnore)
                gateData.RemoveUnitToIgnore(unitToRemove);
        }
    }

    public void UpgradeGates(int upgradeValue, bool isMultiplyingValue)
    {
        foreach (var gate in _gates)
        {
            var view = gate.Key;
            var data = gate.Value;

            if (isMultiplyingValue)
                data.MultiplyMultiplyingValue(upgradeValue);
            else
                data.IncreaseMultiplyingValue(upgradeValue);

            view.OnGateUpgrade(data.CurrentMultiplyingValue);
        }
    }

    private void MultiplyUnits(IMultiplyingGateView gateView, IUnitView unitView)
    {
        GateData gateData = _gates[gateView];

        if (unitView.BattleSide == UnitBattleSide.Player && gateData.UnitsToIgnore.ContainsKey(unitView) == false)
        {
            gateData.AddUnitToIgnore(unitView, _localTime + TimeToIgnoreUnit);

            for (int i = 0; i < _gates[gateView].CurrentMultiplyingValue - 1; i++)
            {
                unitView.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
                position += (UnityEngine.Random.insideUnitSphere * SpawnPositionRandomOffset).ResetY();
                var spawnedUnitView = _unitSpawner.SpawnUnit(position, rotation).View;

                gateData.AddUnitToIgnore(spawnedUnitView, _localTime + TimeToIgnoreUnit);
            }
        }
    }

    public void Dispose()
    {
        foreach (var view in _gates)
            view.Key.OnUnitEntered -= MultiplyUnits;
    }

    public class GateData
    {
        public int CurrentMultiplyingValue { get; private set; }
        public Dictionary<IUnitView, float> UnitsToIgnore { get; private set; }

        public GateData(int initialMultiplyingValue)
        {
            CurrentMultiplyingValue = initialMultiplyingValue;
            UnitsToIgnore = new Dictionary<IUnitView, float>(64);
        }

        public void IncreaseMultiplyingValue(int additionalValue) => CurrentMultiplyingValue += additionalValue;
        public void MultiplyMultiplyingValue(int multiplyValue) => CurrentMultiplyingValue *= multiplyValue;

        public void AddUnitToIgnore(IUnitView unitView, float removeTime) => UnitsToIgnore.Add(unitView, removeTime);
        public void RemoveUnitToIgnore(IUnitView unitView) => UnitsToIgnore.Remove(unitView);
    }
}