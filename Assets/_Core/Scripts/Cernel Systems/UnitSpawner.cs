using UnityEngine;
using System.Collections.Generic;
using System;

public class UnitSpawner : IUpdatable, IDisposable
{
    private UnitConfig _unitConfig;
    private Transform _unitsContainer;

    private HashSet<UnitBase> _activeUnitsPool = new HashSet<UnitBase>(1024);
    private Stack<UnitBase> _deactivatedUnitsPool = new Stack<UnitBase>(1024);

    public event Action<UnitBase> OnUnitSpawned;
    public event Action<UnitBase> OnUnitDespawned;

    public UnitSpawner(UnitConfig unitConfig, Transform unitsContainer = null)
    {
        _unitConfig = unitConfig;
        _unitsContainer = unitsContainer;
    }

    public void UpdateLogic(float deltaTime)
    {
        foreach (var unit in _activeUnitsPool)
            unit.UpdateLogic(deltaTime);
    }

    public UnitBase SpawnUnit(Vector3 position, Quaternion rotation, Action<UnitBase> beforeActivateAction = null)
    {
        UnitBase unit = ExtractFreeUnit();

        unit.Movement.SetDirection(rotation);
        unit.Movement.TeleportToPosition(position);

        _activeUnitsPool.Add(unit);
        
        beforeActivateAction?.Invoke(unit);

        unit.EnableUnit();

        OnUnitSpawned?.Invoke(unit);
        return unit;
    }

    public void DeactivateUnit(UnitBase unit, bool triggerOnDespawnedEvent = true)
    {
        unit.DisableUnit();
        unit.DisableView();
        _activeUnitsPool.Remove(unit);
        _deactivatedUnitsPool.Push(unit);

        if (triggerOnDespawnedEvent)
            OnUnitDespawned?.Invoke(unit);
    }

    public void KillAllActieveUnits()
    {
        foreach (UnitBase unit in _activeUnitsPool)
            unit.Health.TakeDamage(int.MaxValue);
    }

    public void DeactivateAllActieveUnits()
    {
        foreach (UnitBase unit in _activeUnitsPool)
            DeactivateUnit(unit, false);
    }

    private UnitBase ExtractFreeUnit()
    {
        if (_deactivatedUnitsPool.Count <= 0)
            CreateNewUnit();

        return _deactivatedUnitsPool.Pop();
    }

    private void CreateNewUnit()
    {
        UnitView unitView = MonoBehaviour.Instantiate(_unitConfig.UnitViewPrefab, _unitsContainer);
        Health unitHealth = new Health(_unitConfig.MaxHealth);
        IUnitMovement unitMovement = new UnitMovementModel(_unitConfig.MoveSpeed);
        IUnitAttack unitAttack = new UnitAttacker(_unitConfig, unitHealth);

        UnitBase unitBase = new UnitBase(this, unitHealth, unitMovement, unitAttack, _unitConfig);
        unitView.Init(unitBase);
        unitBase.DisableUnit();

        _deactivatedUnitsPool.Push(unitBase);
    }

    public void Dispose()
    {
        foreach (UnitBase unit in _activeUnitsPool)
            unit.Dispose();

        foreach(UnitBase unit in _deactivatedUnitsPool)
            unit.Dispose();
    }
}