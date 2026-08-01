using PrimeTween;
using UnityEngine;

public class GameScenarioDirector : MonoBehaviour
{
    [SerializeField] private UiSystem _uiSystem;
    [SerializeField] private GameScenario _scenario;
    [SerializeField] private bool _runScenarioFromStart;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize() => PrimeTweenConfig.SetTweensCapacity(1024);

    private void Start()
    {
        if (_runScenarioFromStart)
            RunScenario();
    }

    public void SetScenario(GameScenario newScenario) => _scenario = newScenario;
    [ContextMenu("Run Scenarion")]
    public void RunScenario()
    {
        _uiSystem.HideAllScreens();

        UnsubscribeFromScenario(_scenario);
        SubscribeToScenario(_scenario);
        _scenario.PerformScenario();
    }

    public void StopScenarion()
    {
        UnsubscribeFromScenario(_scenario);
        _scenario.FinishScenario();
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
}
