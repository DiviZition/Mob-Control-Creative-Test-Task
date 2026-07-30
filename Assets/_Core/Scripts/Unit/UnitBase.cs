using UnityEngine;

public class UnitBase : MonoBehaviour, IPoolable, IDisablable
{
    [field: SerializeField] public Transform Transform { get; private set; }
    [field: SerializeField] public UnitBattleSide UnitBattleSide { get; private set; }
    [field: SerializeField] public UnitMovement Movement { get; private set; }
    [field: SerializeField] public UnitBaseFX FX { get; private set; }

    public MultiplyingGate IgnoreGate { get; private set; }

    [SerializeField] private UnitDamageDealer _damageDealer;

    public void ActivatePoolable()
    {
        Movement.ResetAgent();
        this.gameObject.SetActive(true);
        FX.PlaySpawnVFX();
    }

    public void DeactivatePoolable()
    {
        this.gameObject.SetActive(false);
    }

    public void Enable()
    {
        Movement.Enable();
        //_damageDealer.Enable();
    }

    public void Disable()
    {
        Movement.Disable();
        //_damageDealer.Disable();
    }

    public void SetGateToIgnore(MultiplyingGate ignoreGate) => IgnoreGate = ignoreGate;
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

public class UnitDamageDealer : IDisablable
{
    public void Disable()
    {
        throw new System.NotImplementedException();
    }

    public void Enable()
    {
        throw new System.NotImplementedException();
    }
}