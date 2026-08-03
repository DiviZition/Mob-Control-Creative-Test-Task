using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
public class EnemyTower : Damageable, IActivatable
{
    [SerializeField] private HordsUnitsSpawner _bossSpawning;
    [SerializeField] private HordsUnitsSpawner _enemySpawning;
    [SerializeField] private Transform _visual;

    private void Start() => RestoreHealth();

    protected override void PerformDeath()
    {
        base.PerformDeath();
        _enemySpawning.KillAllUnits();
        _enemySpawning.StopSpawning();

        _bossSpawning.KillAllUnits();
        _bossSpawning.StopSpawning();

        _visual.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        _enemySpawning.DrawSpawnLine();
        _bossSpawning.DrawSpawnLine();
    }

    public void Enable()
    {
        RestoreHealth();
        _visual.gameObject.SetActive(true);
        _enemySpawning.StartSpawningEnemy();
        _bossSpawning.StartSpawningEnemy();
    }

    public void Disable()
    {
        //_enemySpawning.StopSpawning();
    }
}
