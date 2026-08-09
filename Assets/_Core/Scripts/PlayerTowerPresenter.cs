using System;
using UnityEngine;

public class PlayerTowerView : MonoBehaviour
{
    [field: SerializeField] public Collider Collider { get; private set; }
    [field: SerializeField] public DamageableView PlayerDamageable { get; private set; }
    [field: SerializeField] public CanonView CanonView { get; private set; }
}

public class PlayerTowerPresenter : IDisposable
{
    private Health _health;
    private PlayerTowerView _view;

    public PlayerTowerPresenter(Health playerHealth, PlayerTowerView view)
    {
        _health = playerHealth;
        _view = view;

        _view.PlayerDamageable.OnDamageTaken += _health.TakeDamage;
    }

    public void DisableCollider() => _view.Collider.enabled = false;
    public void EnableCollider() => _view.Collider.enabled = true;
    public void PlayKickEffect()
    {
        _view.CanonView.CanonHitFeedback.ResetFeedbacks();
        _view.CanonView.CanonHitFeedback.RestoreInitialValues();
        _view.CanonView.CanonHitFeedback.PlayFeedbacks();
    }

    public void Dispose()
    {
        _view.PlayerDamageable.OnDamageTaken -= _health.TakeDamage;
    }
}

public class PlayerTowerModel : IDisposable
{
    public Health Health { get; private set; }
    private PlayerTowerPresenter _presenter;

    public PlayerTowerModel(Health playerHealth, PlayerTowerPresenter presenter)
    {
        Health = playerHealth;
        _presenter = presenter;

        Health.OnDead += OnPlayerKilled;
    }

    public void OnPlayerKilled()
    {
        _presenter.PlayKickEffect();
        _presenter.DisableCollider();
    }

    public void Dispose()
    {
        Health.OnDead -= OnPlayerKilled;
    }
}