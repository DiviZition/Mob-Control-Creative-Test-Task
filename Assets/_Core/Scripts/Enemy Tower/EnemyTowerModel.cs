using System;

public interface IEnemyTowerModel : IUpdatable, IDisposable
{
    public Health Health { get; }
}

public class EnemyTowerModel : IEnemyTowerModel
{
    private HordsUnitsSpawner[] _hordsToSpawn;
    private readonly HordSpawnConfig[] _hordsConfigs;
    public Health Health { get; private set; }

    public EnemyTowerModel(Health health, HordSpawnConfig[] hordsConfigs, UnitSpawnParameters spawnParameters)
    {
        Health = health;
        _hordsConfigs = hordsConfigs;

        _hordsToSpawn = new HordsUnitsSpawner[_hordsConfigs.Length];
        for (int i = 0; i < _hordsConfigs.Length; i++)
            _hordsToSpawn[i] = new HordsUnitsSpawner(_hordsConfigs[i], spawnParameters);
    }

    public void UpdateLogic(float deltaTime)
    {
        foreach (var hordSpawner in _hordsToSpawn)
            hordSpawner.UpdateLogic(deltaTime);
    }

    public void Dispose()
    {
        foreach (var hordSpawner in _hordsToSpawn)
            hordSpawner?.Dispose();
    }
}