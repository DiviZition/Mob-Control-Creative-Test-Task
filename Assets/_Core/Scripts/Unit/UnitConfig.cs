using UnityEngine;

[CreateAssetMenu(fileName = "UnitConfig", menuName = "Configs/UnitConfig")]
public class UnitConfig : ScriptableObject
{
    [field: SerializeField] public UnitView UnitViewPrefab {  get; private set; }

    [field: SerializeField] public float MoveSpeed { get; private set; }

    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public bool ReturnsDamage { get; private set; }
    [field: SerializeField] public UnitBattleSide BattleSide { get; private set; }

    [field: SerializeField] public int Damage { get; private set; }
    [field: SerializeField] public float AttackCooldown { get; private set; }
    [field: SerializeField] public float AttackWindUpDelay { get; private set; }
    [field: SerializeField] public bool PlayAttackAnimation { get; private set; }
}
