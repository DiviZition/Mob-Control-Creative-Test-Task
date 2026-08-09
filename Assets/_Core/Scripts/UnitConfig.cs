using UnityEngine;

[CreateAssetMenu(fileName = "UnitConfig", menuName = "Configs/UnitConfig")]
public class UnitConfig : ScriptableObject
{
    [field: SerializeField] public UnitView UnitViewPrefab {  get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public int MaxHealth { get; private set; }
}
