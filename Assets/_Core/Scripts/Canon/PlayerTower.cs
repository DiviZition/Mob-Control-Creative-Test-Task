using MoreMountains.Feedbacks;
using UnityEngine;

public class PlayerTower : Damageable, IActivatable
{
    [SerializeField] private CanonMovement _canonMovement;
    [SerializeField] private CanonShooter _canonShooter;
    [SerializeField] private Collider _collider;
    [SerializeField] private MMF_Player _canonHitFeedback;

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

    protected override void PerformDeath()
    {
        base.PerformDeath();
        _canonHitFeedback.PlayFeedbacks();
    }
}
