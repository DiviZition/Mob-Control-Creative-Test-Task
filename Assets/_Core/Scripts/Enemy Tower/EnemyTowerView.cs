using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

public class EnemyTowerView : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private Transform _unitSpawnOrigin;
    [SerializeField] private Transform _unitsContainer;
    [SerializeField] private float _unitSpawnPositionRadius;

    [SerializeField] private Collider _collider;
    [SerializeField] private HealthCounterView _healthCounterUpdater;

    [SerializeField] private MMF_Player _onHitEffect;
    [field: SerializeField] public HordSpawnConfig[] EnemyConfigs { get; private set; }

    private IEnemyTowerModel _enemyTowerModel;

    public void Init(IEnemyTowerModel towerModel)
    {
        _enemyTowerModel = towerModel;
        _enemyTowerModel.Health.OnHealthChanged += TryPlayHitEffectOnHealthChanged;
        _enemyTowerModel.Health.OnDead += Deactivate;
        _healthCounterUpdater.Init(towerModel.Health);
    }

    private void OnDestroy()
    {
        if (_enemyTowerModel != null)
        {
            _enemyTowerModel.Health.OnHealthChanged -= TryPlayHitEffectOnHealthChanged;
            _enemyTowerModel.Health.OnDead -= Deactivate;
        }
    }

    public UnitSpawnParameters GetSpawnParameters() => new UnitSpawnParameters(_unitSpawnOrigin, _unitSpawnPositionRadius, _unitsContainer);

    public void TryPlayHitEffectOnHealthChanged(int maxHealth, int newHealth)
    {
        if (maxHealth == newHealth)
            return;

        _onHitEffect.RestoreInitialValues();
        _onHitEffect.ResetFeedbacks();
        _onHitEffect.PlayFeedbacks();
    }

    public void Deactivate()
    {
        _transform.gameObject.SetActive(false);
        _collider.enabled = false;
    }

    public void Activate()
    {
        _transform.gameObject.SetActive(true);
        _collider.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_unitSpawnOrigin.position, _unitSpawnPositionRadius);
    }
}
