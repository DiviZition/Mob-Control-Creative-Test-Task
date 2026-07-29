using UnityEngine;
using System.Collections.Generic;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private UnitBase _unitPrefab;
    [SerializeField] private Transform _unitsContainer;

    private HashSet<IPoolable> _activeUnitsPool = new HashSet<IPoolable>(1024);
    private Stack<IPoolable> _deactivatedUnitsPool = new Stack<IPoolable>(1024);

    public void SpawnUnit(Vector3 position, Quaternion rotation)
    {
        IPoolable unit = ExtractFreeUnit();
        unit.Transform.position = position;
        unit.Transform.localRotation = rotation;

        _activeUnitsPool.Add(unit);
        unit.ActivatePoolable();
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
        IPoolable newUnit = MonoBehaviour.Instantiate(_unitPrefab, _unitsContainer);
        _deactivatedUnitsPool.Push(newUnit);
    }
}

public interface IPoolable
{
    public Transform Transform { get; }
    public void ActivatePoolable();
    public void DeactivatePoolable();
}
