using MoreMountains.Feedbacks;
using UnityEngine;

public class RoadBlock : MonoBehaviour, IDamageable
{
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public bool ReturnsDamage { get; private set; }
    [field: SerializeField] public UnitBattleSide BattleSide { get; private set; }
    [field: SerializeField] public Transform Transform { get; private set; }
    [SerializeField] private MMF_Player _hitFeedback;
    [SerializeField] private HealthCounterView _healthCounter;

    public Health Health { get; private set; }

    public bool IsDead => Health.IsDead;
    private void OnValidate() => Transform ??= transform;

    private void Start()
    {
        Health = new Health(MaxHealth);
        _healthCounter.Init(Health);
        Health.OnDead += OnDead;
    }

    private void OnDestroy() => Health.OnDead -= OnDead;

    public void TakeDamage(int damage) => Health.TakeDamage(damage);

    private void OnDead() => gameObject.SetActive(false);
}
