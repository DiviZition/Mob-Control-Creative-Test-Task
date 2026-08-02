using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using R3;
using System;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private UnitBase _unitPrefab;
    [SerializeField] private Transform _unitsContainer;

    private HashSet<IPoolable> _activeUnitsPool = new HashSet<IPoolable>(1024);
    private Stack<IPoolable> _deactivatedUnitsPool = new Stack<IPoolable>(1024);

    public Subject<UnitBase> OnUnitSpawned {  get; private set; } = new Subject<UnitBase>();
    public Subject<UnitBase> OnUnitDespawned {  get; private set; } = new Subject<UnitBase>();

    public UnitBase SpawnUnit(Vector3 position, Quaternion rotation, Action<UnitBase> beforeActivateAction = null)
    {
        IPoolable poolableUnit = ExtractFreeUnit();
        poolableUnit.Transform.position = position;
        poolableUnit.Transform.localRotation = rotation;

        _activeUnitsPool.Add(poolableUnit);
        
        UnitBase unitBase = poolableUnit as UnitBase;
        beforeActivateAction?.Invoke(unitBase);

        poolableUnit.ActivatePoolable();

        OnUnitSpawned.OnNext(unitBase);
        return unitBase;
    }

    public void DeactivateUnit(UnitBase unit)
    {
        unit.DeactivatePoolable();
        _activeUnitsPool.Remove(unit);
        _deactivatedUnitsPool.Push(unit);
        OnUnitDespawned.OnNext(unit);
    }

    public void KillAllActieveUnits()
    {
        foreach (IDamageable unitDamageable in _activeUnitsPool)
            unitDamageable.TakeDamage(int.MaxValue);
    }

    private IPoolable ExtractFreeUnit()
    {
        if (_deactivatedUnitsPool.Count <= 0)
            CreateNewUnit();

        return _deactivatedUnitsPool.Pop();
    }

    private void CreateNewUnit()
    {
        UnitBase newUnit = MonoBehaviour.Instantiate(_unitPrefab, _unitsContainer);
        newUnit.SetUnitSpawner(this);
        newUnit.DeactivatePoolable();
        _deactivatedUnitsPool.Push(newUnit);
    }
}

public interface IPoolable
{
    public Transform Transform { get; }
    public void ActivatePoolable();
    public void DeactivatePoolable();
}
