using MoreMountains.Feedbacks;
using System;
using UnityEngine;

public class PlayerTowerView : MonoBehaviour, IDamageable
{
    [SerializeField] private Collider _collider;
    [field: SerializeField] public Transform Transform { get; private set; }
    [field: SerializeField] public bool ReturnsDamage {  get; private set; }
    [field: SerializeField] public MMF_Player CanonHitFeedback { get; private set; }

    private Health _playerHealth;
    public UnitBattleSide BattleSide => UnitBattleSide.Player;
    public bool IsDead => _playerHealth.IsDead;

    public void Init(Health playerHealth)
    {
        _playerHealth = playerHealth;

        _playerHealth.OnDead += PerformPlayrViewDead;
    }

    private void OnDestroy()
    {
        if (_playerHealth != null)
            _playerHealth.OnDead -= PerformPlayrViewDead;
    }
    public void TakeDamage(int damage) => _playerHealth.TakeDamage(damage);
    
    public void PerformPlayrViewDead()
    {
        _collider.enabled = false;
        PlayKickEffect();
    }

    public void PlayKickEffect()
    {
        CanonHitFeedback.ResetFeedbacks();
        CanonHitFeedback.RestoreInitialValues();
        CanonHitFeedback.PlayFeedbacks();
    }
}