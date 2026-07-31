using UnityEngine;
using UnityEngine.InputSystem;

public class GameScenarioExecutor : MonoBehaviour
{
    [SerializeField] private UiSystem _uiSystem;

    [SerializeField] private GameScenario _scenario;

    public void SetScenario(GameScenario newScenario) => _scenario = newScenario;
    [ContextMenu("Run Scenarion")]
    public void RunScenario()
    {
        _uiSystem.HideAllScreens();

        UnsubscribeFromScenario(_scenario);
        SubscribeToScenario(_scenario);
        _scenario.PerformScenario();
    }

    private void OnPlayerLost()
    {
        _uiSystem.ShowDefeatedScreen();
    }

    private void OnPlayerWon()
    {

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
