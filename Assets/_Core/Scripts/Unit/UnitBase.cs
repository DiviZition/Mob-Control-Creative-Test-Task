using UnityEngine;

public class UnitBase : MonoBehaviour, IPoolable
{
    [field: SerializeField] public Transform Transform { get; private set; }
    [field: SerializeField] public UnitBattleSide UnitBattleSide { get; private set; }

    [SerializeField] private UnitDamageDealer _damageDealer;
    [SerializeField] private UnitMovement _movement;

    public void ActivatePoolable()
    {
        _movement.ResetAgent();
        this.gameObject.SetActive(true);
    }

    public void DeactivatePoolable()
    {
        this.gameObject.SetActive(false);
        Debug.Log($"Deactivating unit: [{this.name}]");
    }
}

public enum UnitBattleSide
{
    Player,
    Enemy,
}

public class Damageable : MonoBehaviour, IDamageable
{

}

internal interface IDamageable
{
}

public class UnitDamageDealer
{

}