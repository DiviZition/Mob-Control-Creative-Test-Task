using UnityEngine;

public class PlayerTower : Damageable, IDisablable
{
    [SerializeField] private CanonMovement _canonMovement;
    [SerializeField] private CanonShooter _canonShooter;
    [SerializeField] private Collider _collider;

    public void Enable()
    {
        RestoreHealth();
        _canonMovement.enabled = true;
        _canonShooter.enabled = true;
        _collider.enabled = true;
    }

    public void Disable()
    {
        _canonMovement.enabled = false;
        _canonShooter.enabled = false;
        _collider.enabled = false;
    }
}
