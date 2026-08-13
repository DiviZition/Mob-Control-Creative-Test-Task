using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class HordsUnitsSpawner : IUpdatable, IDisposable
{
    private HordSpawnConfig _config;
    private UnitSpawner _unitSpawner;
    private UnitSpawnParameters _parameters;
    private float _timer;
    public bool IsStopped { get; set; }

    private Vector3 SpawnPosition => _parameters.UnitSpawnPosition + (Random.insideUnitSphere * _parameters.SpawnPositionOffset).ResetY();

    public HordsUnitsSpawner(HordSpawnConfig hordConfig, UnitSpawnParameters parameters)
    {
        _config = hordConfig;
        _parameters = parameters;
        _unitSpawner = new UnitSpawner(hordConfig, parameters.UnitsContainer);
    }

    public void UpdateLogic(float deltaTime)
    {
        _timer += deltaTime;
        _unitSpawner.UpdateLogic(deltaTime);

        if (IsStopped == true || _timer < _config.SpawnTickDelay)
            return;

        _timer = 0;
        _unitSpawner.SpawnUnit(SpawnPosition, _parameters.UnitInitialDirection);
    }

    public void Dispose() => _unitSpawner.Dispose();
}
