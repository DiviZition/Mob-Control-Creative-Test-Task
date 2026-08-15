using PrimeTween;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ModelsUpdater))]
public class GameInit : MonoBehaviour, IDisposable
{
    public static GameInit Instance {  get; private set; }

    [SerializeField] private bool _autoRun;
    [SerializeField] private ModelsUpdater _updater;
    [SerializeField] private UiSystem _uiSystem;

    [SerializeField] private CanonView _canonView;
    [SerializeField] private PlayerTowerView _playerTowerView;
    [SerializeField] private EnemyTowerView _enemyTowerView;

    [SerializeField] private MultiplyingGateSystemViews _gatesViesProvider;

    private IInputProvider _inputProvider;
    private List<IDisposable> _disposables = new(8);

    public UnitSpawner PlayerUnitSpawner { get; private set; }
    private Health _playerHealth;
    private Health _enemyHealth;

    [RuntimeInitializeOnLoadMethod]
    public static void RunTimeInitialization()
    {
        PrimeTweenConfig.SetTweensCapacity(2048);
    }

    private void Start()
    {
        if (_autoRun)
            InitGame();
    }

    public void InitGame()
    {
        Debug.Log("Game Init Called");
        Dispose();
        _inputProvider = new KeyboardInput();
        _disposables.Add(_inputProvider);

        UnitSpawner playerUnitSpawner = new UnitSpawner(_canonView.PlayerUnitConfig);
        _updater.Register(playerUnitSpawner);
        _playerHealth = new Health(1);

        CanonModel canonModel = new CanonModel(_inputProvider, _canonView.CanonConfig, playerUnitSpawner, _canonView.Transform.position);
        _canonView.Init(canonModel);
        _updater.Register(canonModel);
        _disposables.Add(playerUnitSpawner);

        _playerTowerView.Init(_playerHealth);

        _enemyHealth = new Health(100);
        EnemyTowerModel enemyTowerModel = new EnemyTowerModel(_enemyHealth, _enemyTowerView.EnemyConfigs, _enemyTowerView.GetSpawnParameters());
        _enemyTowerView.Init(enemyTowerModel);
        _updater.Register(enemyTowerModel);
        _disposables.Add(enemyTowerModel);

        MultiplyingGateSystem multiplyingGateSystem = new MultiplyingGateSystem(playerUnitSpawner);
        _gatesViesProvider.Init(multiplyingGateSystem);
        _updater.Register(multiplyingGateSystem);

        _playerHealth.OnDead += FinishGame_EnemyWin;
        _enemyHealth.OnDead += FinishGame_PlayerWin;
    }

    public void FinishGame_EnemyWin()
    {
        _uiSystem.ShowDefeatedScreen();
        _updater.UnregisterAll();
    }

    public void FinishGame_PlayerWin()
    {
        _uiSystem.ShowYouWonScreen();
        _updater.UnregisterAll();
    }

    public void Dispose()
    {
        if (_playerHealth != null)
            _playerHealth.OnDead -= FinishGame_EnemyWin;
        if (_enemyHealth != null)
            _enemyHealth.OnDead -= FinishGame_PlayerWin;

        _updater.UnregisterAll();
        foreach (var disposable in _disposables)
            disposable.Dispose();
    }
}