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

    [SerializeField] private MultiplyingGateSystemViews _gatesViesProvider;

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
            GameObject.Destroy(Instance);
        //Destroying the old instance, becouse of disabled domain recompilation.
        //Statics will not be cilled, so we're replacing the null with the new instance.
        //Works for prototype, but needs to be overhauled if we're going to go farther.

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void InitGame()
    {
        _inputProvider = new KeyboardInput();

        UnitSpawner playerUnitSpawner = new UnitSpawner(_canonView.PlayerUnitConfig);
        IHealth playerHealth = new Health(1);

        CanonModel canonModel = new CanonModel(_inputProvider, _canonView.CanonConfig, PlayerUnitSpawner);
        _updater.Register(canonModel);
        _disposables.Add(playerUnitSpawner);

        _playerTowerView.Init(playerHealth);

        Health enemyHealth = new Health(100);
        EnemyTowerModel enemyTowerModel = new EnemyTowerModel(enemyHealth, _enemyTowerView.EnemyConfigs, _enemyTowerView.GetSpawnParameters());
        _enemyTowerView.Init(enemyTowerModel);
        _updater.Register(enemyTowerModel);
        _disposables.Add(enemyTowerModel);

        MultiplyingGateSystem multiplyingGateSystem = new MultiplyingGateSystem(playerUnitSpawner);
        _gatesViesProvider.Init(multiplyingGateSystem);
        _updater.Register(multiplyingGateSystem);
    }
}