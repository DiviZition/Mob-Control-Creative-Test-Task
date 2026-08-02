using PrimeTween;
using UnityEngine;

public class GameScenarioDirector : MonoBehaviour
{
    [SerializeField] private UiSystem _uiSystem;
    [SerializeField] private GameScenario _scenario;
    [SerializeField] private bool _runScenarioOnStart;

    private GameScenario _currentScenario;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize() => PrimeTweenConfig.SetTweensCapacity(1024);

    private void Start()
    {
        if (_runScenarioOnStart)
            RunScenario();
    }

    public void RunScenario()
    {
        _uiSystem.HideAllScreens();

        UnsubscribeFromScenario(_currentScenario);
        SubscribeToScenario(_scenario);
        _currentScenario = _scenario;

        _scenario.PerformScenario();
    }

    public void StopScenarion()
    {
        UnsubscribeFromScenario(_scenario);
        _scenario.StopScenario();
    }

    private void OnPlayerLost()
    {
        _uiSystem.ShowDefeatedScreen();
        StopScenarion();
    }

    private void OnPlayerWon()
    {
        _uiSystem.ShowYouWonScreen();
        StopScenarion();
    }

    private void UnsubscribeFromScenario(GameScenario scenario)
    {
        if (scenario != null)
        {
            scenario.OnPlayerLost -= OnPlayerLost;
            scenario.OnPlayerWon -= OnPlayerWon;
        }
    }

    private void SubscribeToScenario(GameScenario scenario)
    {
        if (scenario != null)
        {
            scenario.OnPlayerLost += OnPlayerLost;
            scenario.OnPlayerWon += OnPlayerWon;
        }
    }

    private void OnDestroy() => UnsubscribeFromScenario(_scenario);
}
