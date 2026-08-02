using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
public class EnemyTower : Damageable, IActivatable
{
    [SerializeField] private HordsUnitsSpawner _enemySpawning;
    [SerializeField] private Transform _visual;

    private void Start() => RestoreHealth();

    protected override void PerformDeath()
    {
        base.PerformDeath();
        _enemySpawning.KillAllUnits();
        _enemySpawning.StopSpawning();

        _visual.gameObject.SetActive(false);
    }

    private void OnDrawGizmos() => _enemySpawning.DrawSpawnLine();

    public void Enable()
    {
        RestoreHealth();
        _visual.gameObject.SetActive(true);
        _enemySpawning.StartSpawningEnemy();
    }

    public void Disable()
    {
        //_enemySpawning.StopSpawning();
    }
}
