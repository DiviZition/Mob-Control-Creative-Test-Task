using MoreMountains.Feedbacks;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "EnemyHordConfig", menuName = "Configs/EnemyHordConfig")]
public class HordSpawnConfig : UnitConfig
{
    [field: SerializeField] public float TimeToMaxPower { get; private set; }
    [field: SerializeField] public float SpawnTickDelay { get; private set; }
    [field: SerializeField] public AnimationCurve UnitsPerSpawnTick { get; private set; }
}

public struct UnitSpawnParameters
{
    public Vector3 UnitSpawnPosition { get; private set; }
    public Quaternion UnitInitialDirection { get; private set; }
    public float SpawnPositionOffset { get; private set; }

    public UnitSpawnParameters(Transform reference, float spawnPositionOffset) 
        : this(reference.position, reference.rotation, spawnPositionOffset) { }
    public UnitSpawnParameters(Vector3 unitSpawnPosition, Quaternion unitInitialDirection, float spawnPositionOffset)
    {
        UnitSpawnPosition = unitSpawnPosition;
        UnitInitialDirection = unitInitialDirection;
        SpawnPositionOffset = spawnPositionOffset;
    }
}

public class EnemyTowerView : MonoBehaviour
{
    [field: SerializeField] public Transform Transform { get; private set; }
    [field: SerializeField] public Transform UnitSpawnOrigin { get; private set; }
    [field: SerializeField] public Transform UnitsContainer { get; private set; }
    [field: SerializeField] public float UnitSpawnPositionOffset { get; private set; }

    [field: SerializeField] public DamageableView Damageable { get; private set; }
    [field: SerializeField] public Collider Collider { get; private set; }
    
    [field: SerializeField] public MMF_Player OnHitEffect { get; private set; }
    [field: SerializeField] public HordSpawnConfig[] EnemyConfigs { get; private set; } 

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(UnitSpawnOrigin.position, UnitSpawnPositionOffset);
    }
}

public class EnemyTowerPresenter : IDisposable
{
    private EnemyTowerView _view;
    private Health _health;
    public UnitSpawnParameters SpawnParameters { get; private set; }

    public HordSpawnConfig[] EnemyConfigs => _view.EnemyConfigs;
    public Transform UnitsContainer => _view.UnitsContainer;

    public EnemyTowerPresenter(EnemyTowerView view, Health health)
    {
        _view = view;
        _health = health;

        _view.Damageable.OnDamageTaken += _health.TakeDamage;
        SpawnParameters = new UnitSpawnParameters(_view.UnitSpawnOrigin, _view.UnitSpawnPositionOffset);
    }

    public void PlayHitEffect()
    {
        _view.OnHitEffect.RestoreInitialValues();
        _view.OnHitEffect.ResetFeedbacks();
        _view.OnHitEffect.PlayFeedbacks();
    }

    public void Deactivate()
    {
        _view.Transform.gameObject.SetActive(false);
        _view.Collider.enabled = false;
    }

    public void Activate()
    {
        _view.Transform.gameObject.SetActive(true);
        _view.Collider.enabled = true;
    }

    public void Dispose() => _view.Damageable.OnDamageTaken -= _health.TakeDamage;
}

public class EnemyTowerModel : IUpdatable, IDisposable
{
    private HordsUnitsSpawner[] _hordsToSpawn;
    private EnemyTowerPresenter _presenter;
    private Health _health;

    public EnemyTowerModel(EnemyTowerPresenter presenter, Health health)
    {
        _presenter = presenter;
        _health = health;
        _health.OnDead += PerformDeath;

        _hordsToSpawn = new HordsUnitsSpawner[presenter.EnemyConfigs.Length];
        for (int i = 0; i < presenter.EnemyConfigs.Length; i++)
            _hordsToSpawn[i] = new HordsUnitsSpawner(presenter.EnemyConfigs[i], presenter.UnitsContainer, presenter.SpawnParameters);
    }

    public void UpdateLogic(float deltaTime)
    {
        foreach (var hordSpawner in _hordsToSpawn)
            hordSpawner.UpdateLogic(deltaTime);
    }

    public void PerformDeath() => _presenter.Deactivate();

    public void Dispose()
    {
        _health.OnDead -= PerformDeath;

        foreach (var hordSpawner in _hordsToSpawn)
            hordSpawner?.Dispose();
    }
}

public class HordsUnitsSpawner : IUpdatable, IDisposable
{
    private HordSpawnConfig _config;
    private UnitSpawner _unitSpawner;
    private UnitSpawnParameters _parameters;
    private float _timer;
    public bool IsStopped { get; set; }

    private Vector3 SpawnPosition => _parameters.UnitSpawnPosition + (Random.insideUnitSphere * _parameters.SpawnPositionOffset).ResetY();

    public HordsUnitsSpawner(HordSpawnConfig hordConfig, Transform unitsContainer, UnitSpawnParameters parameters)
    {
        _config = hordConfig;
        _parameters = parameters;
        _unitSpawner = new UnitSpawner(hordConfig, unitsContainer);
    }

    public void UpdateLogic(float deltaTime)
    {
        _timer += deltaTime;
        _unitSpawner.UpdateLogic(deltaTime);

        if (IsStopped == true || _timer < _config.SpawnTickDelay)
            return;

        _timer = 0;
        _unitSpawner.SpawnUnit(SpawnPosition, _parameters.UnitInitialDirection);
    }

    public void Dispose() => _unitSpawner.Dispose();
}
