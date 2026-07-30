using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private UnitBase _unitPrefab;
    [SerializeField] private Transform _unitsContainer;

    private HashSet<IPoolable> _activeUnitsPool = new HashSet<IPoolable>(1024);
    private Stack<IPoolable> _deactivatedUnitsPool = new Stack<IPoolable>(1024);

    public UnitBase SpawnUnit(Transform reference, bool activateUnit = true) => SpawnUnit(reference.position, reference.localRotation, activateUnit);
    public UnitBase SpawnUnit(Vector3 position, Quaternion rotation, bool activateUnit = true)
    {
        IPoolable unit = ExtractFreeUnit();
        unit.Transform.position = position;
        unit.Transform.localRotation = rotation;

        _activeUnitsPool.Add(unit);
        
        if (activateUnit == true)
            unit.ActivatePoolable();

        return unit as UnitBase;
    }

    public void DeactivateUnit(UnitBase unit)
    {
        unit.DeactivatePoolable();
        _activeUnitsPool.Remove(unit);
        _deactivatedUnitsPool.Push(unit);
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
