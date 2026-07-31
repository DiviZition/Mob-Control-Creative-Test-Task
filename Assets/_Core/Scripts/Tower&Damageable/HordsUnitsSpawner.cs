using R3;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class HordsUnitsSpawner
{
    [SerializeField] private UnitSpawner _enemySpawner;

    [SerializeField] private float _timeToMaxPower;
    [SerializeField] private int _enemySpawnPerSecond;
    [SerializeField] private float _enemySpawnDelay;
    [SerializeField] private AnimationCurve _enemySpawnedMultiplyer;

    [SerializeField] private Transform _unitsInitialDirection;
    [SerializeField] private Transform _spawnLeftEndPoint;
    [SerializeField] private float _spawnLength;

    private IDisposable _enemySpawnMachine;
    private float _spawnStartTime;

    [ContextMenu("Start Spawning")]
    public void StartSpawningEnemy(bool resetProgress = true)
    {
        if (resetProgress)
            _spawnStartTime = Time.time;
            
        StopSpawning();
        _enemySpawnMachine = Observable
            .Interval(TimeSpan.FromSeconds(_enemySpawnDelay))
            .Subscribe(_ => CalculateAndSpawnEnemy());
    }

    public void StopSpawning() => _enemySpawnMachine?.Dispose();
    public void KillAllUnits() => _enemySpawner.KillAllActieveUnits(); 

    private void CalculateAndSpawnEnemy()
    {
        float progress = Mathf.InverseLerp(_spawnStartTime, _spawnStartTime + _timeToMaxPower, Time.time);
        int enemiesToSpawn = _enemySpawnPerSecond * (int)_enemySpawnedMultiplyer.Evaluate(progress);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 spawnPosition = _spawnLeftEndPoint.position + _spawnLeftEndPoint.right * Random.Range(0, _spawnLength);
            _enemySpawner.SpawnUnit(spawnPosition, _unitsInitialDirection.rotation);
        }
    }

    public void DrawSpawnLine()
    {
        if (_spawnLeftEndPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(_spawnLeftEndPoint.position, _spawnLeftEndPoint.position + _spawnLeftEndPoint.right * _spawnLength);
    }
}
