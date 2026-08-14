using UnityEngine;
using System.Collections.Generic;
using System;

public class UnitSpawner : IUpdatable, IDisposable
{
    private UnitConfig _unitConfig;
    private Transform _unitsContainer;

    private HashSet<IUnitModel> _activeUnitsPool = new HashSet<IUnitModel>(1024);
    private Stack<IUnitModel> _deactivatedUnitsPool = new Stack<IUnitModel>(1024);

    private int _nextUnitsId;

    public event Action<IUnitModel> OnUnitSpawned;
    public event Action<IUnitModel> OnUnitDespawned;

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

    public IUnitModel SpawnUnit(Vector3 position, Quaternion rotation)
    {
        IUnitModel unit = ExtractFreeUnit();

        unit.Movement.SetDirection(rotation);
        unit.Movement.TeleportToPosition(position);

        _activeUnitsPool.Add(unit);
        
        unit.EnableUnit();

        OnUnitSpawned?.Invoke(unit);
        return unit;
    }

    public void DeactivateUnit(IUnitModel unit, bool triggerOnDespawnedEvent = true)
    {
        unit.DisableUnit();
        _activeUnitsPool.Remove(unit);
        _deactivatedUnitsPool.Push(unit);

        if (triggerOnDespawnedEvent)
            OnUnitDespawned?.Invoke(unit);
    }

    public void KillAllActieveUnits()
    {
        foreach (IUnitModel unit in _activeUnitsPool)
            unit.Health.TakeDamage(int.MaxValue);
    }

    public void DeactivateAllActieveUnits()
    {
        foreach (IUnitModel unit in _activeUnitsPool)
            DeactivateUnit(unit, false);
    }

    private IUnitModel ExtractFreeUnit()
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

        IUnitModel unitBase = new UnitBase(this, unitHealth, unitMovement, unitAttack, _unitConfig, _nextUnitsId);
        unitView.Init(unitBase);
        unitView.Disable();
        unitBase.DisableUnit();

        _nextUnitsId++;
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