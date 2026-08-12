using UnityEngine;
using System;
using TMPro;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using System.Linq;

public interface IMultiplyingGateView
{
    public int InitialMultiplyer { get; }
    public event Action<int, IUnitModel, Vector3, Quaternion> OnUnitEntered;

    public void OnGateUpgrade(int newMultiplyingValue);
}

public class MultiplyingGateView : MonoBehaviour, IMultiplyingGateView
{
    [field: SerializeField] public int InitialMultiplyer { get; private set; }
    [field: SerializeField] public TMP_Text GatesXValueText { get; private set; }
    [field: SerializeField] public MMF_Player GatesUpgradeFeedback { get; private set; }

    public event Action<int, IUnitModel, Vector3, Quaternion> OnUnitEntered;
    public int GateViewKey { get; private set; }

    public void SetKey(int key) => GateViewKey = key;
    public void OnGateUpgrade(int newMultiplyingValue)
    {
        GatesUpgradeFeedback.RestoreInitialValues();
        GatesUpgradeFeedback.ResetFeedbacks();
        GatesUpgradeFeedback.PlayFeedbacks();

        GatesXValueText.text = newMultiplyingValue.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UnitView unitView))
            OnUnitEntered?.Invoke(GateViewKey, unitView.UnitModel, unitView.Transform.position, unitView.Transform.localRotation);
    }
}

public class MultiplyingGateSystemViews : MonoBehaviour
{
    [SerializeField] private MultiplyingGateView[] _gatesViews;
    [SerializeField] private GateUpgradeView[] _gatesUpgrades;

    private IMultiplyingGateSystem _gatesSystem;

    public void Init(IMultiplyingGateSystem gatesSystem)
    {
        _gatesSystem = gatesSystem;

        for (int i = 0; i < _gatesViews.Length; i++)
        {
            _gatesSystem.RegisterMultiplyingGate(i, _gatesViews[i].InitialMultiplyer);
            _gatesViews[i].OnUnitEntered += _gatesSystem.MultiplyUnits;
        }

        foreach (var upgrade in _gatesUpgrades)
            upgrade.OnUpgradePicked += _gatesSystem.UpgradeAllGates;
    }

    private void OnDestroy()
    {
        foreach (var gate in _gatesViews)
            gate.OnUnitEntered -= _gatesSystem.MultiplyUnits;

        foreach (var upgrade in _gatesUpgrades)
            upgrade.OnUpgradePicked -= _gatesSystem.UpgradeAllGates;
    }
}

public interface IMultiplyingGateSystem : IUpdatable
{
    public void RegisterMultiplyingGate(int gateKey, int multiplyingValue);
    public void UpgradeAllGates(int upgradeValue, bool isMultiplyingValue);
    public void MultiplyUnits(int gateKey, IUnitModel unitBase, Vector3 spawnPosition, Quaternion spawnRotation);
}

public class MultiplyingGateSystem : IMultiplyingGateSystem
{
    private UnitSpawner _unitSpawner;
    private Dictionary<int, GateData> _gates = new (8);
    private float _localTime;
    private float _nextUpdateTime;
    private List<IUnitModel> _unitsToRemoveFromIgnore = new(32);

    private const float TimeToIgnoreUnit = 0.5f;
    private const float UpdateThreashold = 0.1f;
    private const float SpawnPositionRandomOffset = 0.1f;

    public event Action<int, int> OnGatesUpgraded;

    public MultiplyingGateSystem(UnitSpawner unitSpawner)
    {
        _unitSpawner = unitSpawner;
    }

    public void UpdateLogic(float deltaTime)
    {
        _localTime += deltaTime;
        if (_localTime < _nextUpdateTime) 
            return;

        _nextUpdateTime = _localTime + UpdateThreashold;
        RemoveExpiredUnitsFromIgnoreList();
    }

    public void RegisterMultiplyingGate(int gateKey, int multiplyingValue)
    {
        GateData newGateData = new GateData(multiplyingValue);
        _gates.Add(gateKey, newGateData);
    }

    public void UpgradeAllGates(int upgradeValue, bool isMultiplyingValue)
    {
        foreach (var gate in _gates)
        {
            var key = gate.Key;
            var data = gate.Value;

            if (isMultiplyingValue)
                data.MultiplyMultiplyingValue(upgradeValue);
            else
                data.IncreaseMultiplyingValue(upgradeValue);

            OnGatesUpgraded?.Invoke(key, data.CurrentMultiplyingValue);
        }
    }

    public void MultiplyUnits(int gateKey, IUnitModel unitBase, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GateData gateData = _gates[gateKey];

        if (unitBase.Config.BattleSide == UnitBattleSide.Player && gateData.UnitsToIgnore.ContainsKey(unitBase) == false)
        {
            gateData.AddUnitToIgnore(unitBase, _localTime + TimeToIgnoreUnit);

            for (int i = 0; i < gateData.CurrentMultiplyingValue - 1; i++)
            {
                spawnPosition += (UnityEngine.Random.insideUnitSphere * SpawnPositionRandomOffset).ResetY();
                var spawnedUnit = _unitSpawner.SpawnUnit(spawnPosition, spawnRotation);

                gateData.AddUnitToIgnore(spawnedUnit, _localTime + TimeToIgnoreUnit);
            }
        }
    }

    private void RemoveExpiredUnitsFromIgnoreList()
    {
        foreach (var view in _gates)
        {
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

    public class GateData
    {
        public int CurrentMultiplyingValue { get; private set; }
        public Dictionary<IUnitModel, float> UnitsToIgnore { get; private set; }

        public GateData(int initialMultiplyingValue)
        {
            CurrentMultiplyingValue = initialMultiplyingValue;
            UnitsToIgnore = new Dictionary<IUnitModel, float>(64);
        }

        public void IncreaseMultiplyingValue(int additionalValue) => CurrentMultiplyingValue += additionalValue;
        public void MultiplyMultiplyingValue(int multiplyValue) => CurrentMultiplyingValue *= multiplyValue;

        public void AddUnitToIgnore(IUnitModel unit, float removeTime) => UnitsToIgnore.Add(unit, removeTime);
        public void RemoveUnitToIgnore(IUnitModel unit) => UnitsToIgnore.Remove(unit);
    }
}