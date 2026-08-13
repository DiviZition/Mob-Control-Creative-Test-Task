using UnityEngine;

[CreateAssetMenu(fileName = "EnemyHordConfig", menuName = "Configs/EnemyHordConfig")]
public class HordSpawnConfig : UnitConfig
{
    [field: SerializeField] public float TimeToMaxPower { get; private set; }
    [field: SerializeField] public float SpawnTickDelay { get; private set; }
    [field: SerializeField] public AnimationCurve UnitsPerSpawnTick { get; private set; }
}
