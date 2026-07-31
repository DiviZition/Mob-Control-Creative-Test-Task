using System;
using UnityEngine;

public class GameScenario : MonoBehaviour
{
    [SerializeField] private Damageable _playerTower;
    [SerializeField] private Damageable _enemyTower;

    //TODO: Make this field respect interfaces via Odin, or a custom serialization logic;
    [SerializeReference] private MonoBehaviour[] _disablableElements;

    public event Action OnPlayerLost;
    public event Action OnPlayerWon;

    public void PerformScenario()
    {
        SubscribeToEvents();
        EnableActieveElements();
    }

    public void FinishScenario()
    {
        UnsubscribeFromEvents();
        DisableActieveElements();
    }

    private void OnPlayerDefeated() => OnPlayerLost?.Invoke();
    private void OnEnemyDefeated() => OnPlayerWon?.Invoke();

    private void EnableActieveElements() => SwitchDisablableElementsEnabled(isEnabled: true);
    private void DisableActieveElements() => SwitchDisablableElementsEnabled(isEnabled: false);
    private void SwitchDisablableElementsEnabled(bool isEnabled)
    {
        foreach (var disablable in _disablableElements)
        {
            if (disablable is IDisablable element)
            {
                if (isEnabled)
                    element.Enable();
                else
                    element.Disable();
            }
        }
    }
    private void UnsubscribeFromEvents()
    {
        _playerTower.OnDead -= OnPlayerDefeated;
        _enemyTower.OnDead -= OnEnemyDefeated;
    }
    private void SubscribeToEvents()
    {
        _playerTower.OnDead += OnPlayerDefeated;
        _enemyTower.OnDead += OnEnemyDefeated;
    }
}