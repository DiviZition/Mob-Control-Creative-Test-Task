using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
public class EnemyTower : Damageable, IDisablable
{
    [SerializeField] private HordsUnitsSpawner _enemySpawning;

    private void Start() => RestoreHealth();

    protected override void PerformDeath()
    {
        base.PerformDeath();
        Disable();
        _enemySpawning.KillAllUnits();

        gameObject.SetActive(false);
    }

    private void OnDrawGizmos() => _enemySpawning.DrawSpawnLine();

    public void Enable()
    {
        RestoreHealth();
        _enemySpawning.StartSpawningEnemy();
    }

    public void Disable()
    {
        //_enemySpawning.StopSpawning();
    }
}
