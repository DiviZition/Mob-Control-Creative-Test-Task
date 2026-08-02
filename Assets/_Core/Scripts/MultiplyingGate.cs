using UnityEngine;
using System;

public class MultiplyingGate : MonoBehaviour
{
    [SerializeField] private int _multiplyingValue;
    [SerializeField] private float _spawnPositionRandomOffset;
    [SerializeField] private UnitBattleSide _whoToMultiply;
    [SerializeField] private UnitSpawner _unitSpawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UnitBase unit) && unit.BattleSide == _whoToMultiply && unit.IgnoreGate != this)
        {
            unit.SetGateToIgnore(this);
            for (int i = 0; i < _multiplyingValue - 1; i++)
            {
                Vector3 spawnPosition = unit.Transform.position;
                spawnPosition += (UnityEngine.Random.insideUnitSphere * _spawnPositionRandomOffset).ResetY();
                
                Action<UnitBase> actionBeforeActivate = (unit) => unit.SetGateToIgnore(this);
                _unitSpawner.SpawnUnit(spawnPosition, unit.Transform.localRotation, actionBeforeActivate);
            }
        }
    }
}