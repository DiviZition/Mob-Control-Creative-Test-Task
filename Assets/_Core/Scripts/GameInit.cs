using PrimeTween;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    public static GameInit Instance {  get; private set; }

    [SerializeField] private ModelsUpdater _updater;
    [SerializeField] private List<IDisposable> _disposables = new(8);

    [SerializeField] private UiSystem _uiSystem;
    [SerializeField] private DamageableView _playerTower;
    [SerializeField] private DamageableView _enemyTower;

    [SerializeField] private CanonView _canonView;
    [SerializeField] private PlayerTowerView _playerTowerView;
    [SerializeField] private EnemyTowerView _enemyTowerView;

    [SerializeField] private MultiplyingGatesViewsProvider _gatesViesProvider;

    private IInputProvider _inputProvider;

    public UnitSpawner PlayerUnitSpawner { get; private set; }

    [RuntimeInitializeOnLoadMethod]
    public static void RunTimeInitialization()
    {
        PrimeTweenConfig.SetTweensCapacity(2048);
    }

    private void Awake()
    {
        if (Instance != null)
            GameObject.Destroy(Instance);//Destroying the old instance becouse of disabled domain recompilation.

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void InitGame()
    {
        _inputProvider = new KeyboardInput();

        UnitSpawner playerUnitSpawner = new UnitSpawner(_canonView.PlayerUnitConfig);

        CanonPresenter canonPresenter = new CanonPresenter(_canonView, _canonView.CanonConfig);
        CanonModel canonModel = new CanonModel(_inputProvider, canonPresenter, _canonView.CanonConfig, PlayerUnitSpawner);
        _updater.Register(canonModel);
        _disposables.Add(playerUnitSpawner);

        Health playerHealth = new Health(1);
        PlayerTowerPresenter playerTowerPresenter = new PlayerTowerPresenter(playerHealth, _playerTowerView);
        PlayerTowerModel playerTowerModel = new PlayerTowerModel(playerHealth, playerTowerPresenter);
        _disposables.Add(playerTowerPresenter);
        _disposables.Add(playerTowerModel);

        Health enemyHealth = new Health(100);
        EnemyTowerPresenter enemyTowerPresenter = new EnemyTowerPresenter(_enemyTowerView, enemyHealth);
        EnemyTowerModel enemyTowerModel = new EnemyTowerModel(enemyTowerPresenter, enemyHealth);
        _updater.Register(enemyTowerModel);
        _disposables.Add(enemyTowerPresenter);
        _disposables.Add(enemyTowerModel);

        MultiplyingGateSystem multiplyingGateSystem = new MultiplyingGateSystem(_gatesViesProvider.GatesViews, playerUnitSpawner);
        _updater.Register(multiplyingGateSystem);
        _disposables.Add(multiplyingGateSystem);

        GatesUpgradeSystem gatesUpgradeSystem = new GatesUpgradeSystem(_gatesViesProvider.GatesUpgrades, multiplyingGateSystem);
        _disposables.Add(gatesUpgradeSystem);
    }
}