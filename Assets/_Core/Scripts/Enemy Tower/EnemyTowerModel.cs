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

public class UnitSpawnParameters
{
    public Vector3 UnitSpawnPosition { get; private set; }
    public Quaternion UnitInitialDirection { get; private set; }
    public float SpawnPositionOffset { get; private set; }
    public Transform UnitsContainer { get; private set; }

    public UnitSpawnParameters() { }
    public UnitSpawnParameters(Transform reference, float spawnPositionOffset, Transform unitContainer)
        : this(reference.position, reference.rotation, spawnPositionOffset, unitContainer) { }
    public UnitSpawnParameters(Vector3 unitSpawnPosition, Quaternion unitInitialDirection, float spawnPositionOffset, Transform unitContainer)
    {
        UnitSpawnPosition = unitSpawnPosition;
        UnitInitialDirection = unitInitialDirection;
        SpawnPositionOffset = spawnPositionOffset;
        UnitsContainer = unitContainer;
    }

    public static UnitSpawnParameters Create() => new UnitSpawnParameters();
    public UnitSpawnParameters WithSpawnPosition(Vector3 spawnPosition)
    {
        UnitSpawnPosition = spawnPosition;
        return this;
    }
    public UnitSpawnParameters WithInitialRotation(Quaternion initialRotation)
    {
        UnitInitialDirection = initialRotation;
        return this;
    }
    public UnitSpawnParameters WithSpawnPositionOffset(float spawnPositionOffset)
    {
        SpawnPositionOffset = spawnPositionOffset;
        return this;
    }
    public UnitSpawnParameters WithContainer(Transform unitContainer)
    {
        UnitsContainer = unitContainer;
        return this;
    }
}

public class EnemyTowerView : MonoBehaviour
{
    [field: SerializeField] public Transform Transform { get; private set; }
    [field: SerializeField] public Transform UnitSpawnOrigin { get; private set; }
    [field: SerializeField] public Transform UnitsContainer { get; private set; }
    [field: SerializeField] public float UnitSpawnPositionOffset { get; private set; }

    [field: SerializeField] public Collider Collider { get; private set; }

    [field: SerializeField] public MMF_Player OnHitEffect { get; private set; }
    [field: SerializeField] public HordSpawnConfig[] EnemyConfigs { get; private set; }

    private IEnemyTowerModel _enemyTowerModel;

    public void Init(IEnemyTowerModel towerModel)
    {
        _enemyTowerModel = towerModel;
        _enemyTowerModel.Health.OnHealthChanged += TryPlayHitEffectOnHealthChanged;
        _enemyTowerModel.Health.OnDead += Deactivate;
    }

    private void OnDestroy()
    {
        _enemyTowerModel.Health.OnHealthChanged -= TryPlayHitEffectOnHealthChanged;
        _enemyTowerModel.Health.OnDead -= Deactivate;
    }

    public UnitSpawnParameters GetSpawnParameters() => new UnitSpawnParameters(UnitSpawnOrigin, UnitSpawnPositionOffset, UnitsContainer);

    public void TryPlayHitEffectOnHealthChanged(int previousHealth, int newHealth)
    {
        if (previousHealth <= newHealth)
            return;

        OnHitEffect.RestoreInitialValues();
        OnHitEffect.ResetFeedbacks();
        OnHitEffect.PlayFeedbacks();
    }

    public void Deactivate()
    {
        Transform.gameObject.SetActive(false);
        Collider.enabled = false;
    }

    public void Activate()
    {
        Transform.gameObject.SetActive(true);
        Collider.enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(UnitSpawnOrigin.position, UnitSpawnPositionOffset);
    }
}

public interface IEnemyTowerModel : IUpdatable, IDisposable
{
    public IHealth Health { get; }
}

public class EnemyTowerModel : IEnemyTowerModel
{
    private HordsUnitsSpawner[] _hordsToSpawn;
    private readonly HordSpawnConfig[] _hordsConfigs;
    public IHealth Health { get; private set; }

    public EnemyTowerModel(IHealth health, HordSpawnConfig[] hordsConfigs, UnitSpawnParameters spawnParameters)
    {
        Health = health;
        _hordsConfigs = hordsConfigs;

        _hordsToSpawn = new HordsUnitsSpawner[_hordsConfigs.Length];
        for (int i = 0; i < _hordsConfigs.Length; i++)
            _hordsToSpawn[i] = new HordsUnitsSpawner(_hordsConfigs[i], spawnParameters);
    }

    public void UpdateLogic(float deltaTime)
    {
        foreach (var hordSpawner in _hordsToSpawn)
            hordSpawner.UpdateLogic(deltaTime);
    }

    public void Dispose()
    {
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

    public HordsUnitsSpawner(HordSpawnConfig hordConfig, UnitSpawnParameters parameters)
    {
        _config = hordConfig;
        _parameters = parameters;
        _unitSpawner = new UnitSpawner(hordConfig, parameters.UnitsContainer);
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
