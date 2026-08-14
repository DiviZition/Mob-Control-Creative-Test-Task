using UnityEngine;
using System;
using System.Collections.Generic;

public interface IMultiplyingGateSystem : IUpdatable
{
    public event Action<int, int> OnGatesUpgraded;

    public void RegisterMultiplyingGate(int gateKey, int multiplyingValue);
    public void UpgradeAllGates(int upgradeValue, bool isMultiplyingValue);
    public void MultiplyUnits(int gateKey, int unitId, UnitBattleSide battleSide, Vector3 spawnPosition, Quaternion spawnRotation);
}

public class MultiplyingGateSystem : IMultiplyingGateSystem
{
    private UnitSpawner _unitSpawner;
    private Dictionary<int, GateData> _gates = new (8);
    private float _localTime;
    private float _nextUpdateTime;
    private List<int> _unitsToRemoveFromIgnore = new(32);

    private const float TimeToIgnoreUnit = 0.5f;
    private const float UpdateThreashold = 0.1f;
    private const float SpawnPositionRandomOffset = 0.5f;

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

    public void MultiplyUnits(int gateKey, int unitId, UnitBattleSide unitsSide, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GateData gateData = _gates[gateKey];

        if (unitsSide == UnitBattleSide.Enemy || gateData.UnitsToIgnore.ContainsKey(unitId) == true)
            return;
        
        gateData.AddUnitToIgnore(unitId, _localTime + TimeToIgnoreUnit);

        for (int i = 0; i < gateData.CurrentMultiplyingValue - 1; i++)
        {
            spawnPosition += (UnityEngine.Random.insideUnitSphere * SpawnPositionRandomOffset).ResetY();
            var spawnedUnit = _unitSpawner.SpawnUnit(spawnPosition, spawnRotation);

            gateData.AddUnitToIgnore(spawnedUnit.ID, _localTime + TimeToIgnoreUnit);
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
        public Dictionary<int, float> UnitsToIgnore { get; private set; }

        public GateData(int initialMultiplyingValue)
        {
            CurrentMultiplyingValue = initialMultiplyingValue;
            UnitsToIgnore = new Dictionary<int, float>(64);
        }

        public void IncreaseMultiplyingValue(int additionalValue) => CurrentMultiplyingValue += additionalValue;
        public void MultiplyMultiplyingValue(int multiplyValue) => CurrentMultiplyingValue *= multiplyValue;

        public void AddUnitToIgnore(int unitId, float removeTime) => UnitsToIgnore.Add(unitId, removeTime);
        public void RemoveUnitToIgnore(int unitId) => UnitsToIgnore.Remove(unitId);
    }
}