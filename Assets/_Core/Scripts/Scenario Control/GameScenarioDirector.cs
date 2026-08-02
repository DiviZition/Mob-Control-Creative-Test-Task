using PrimeTween;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class GameScenarioDirector : SerializedMonoBehaviour
{
    [SerializeField] private UiSystem _uiSystem;
    [SerializeField] private Damageable _playerTower;
    [SerializeField] private Damageable _enemyTower;

    //TODO: Make this field respect interfaces via Odin, or a custom serialization logic;
    [SerializeField] private IActivatable[] _disablableElements;

    [RuntimeInitializeOnLoadMethod]
    public static void RunTimeInitialization()
    {
        PrimeTweenConfig.SetTweensCapacity(2048);
    }

    public void PerformScenario()
    {
        _uiSystem.HideAllScreens();
        SubscribeToEvents();
        EnableActieveElements();
    }

    public void StopScenario()
    {
        UnsubscribeFromEvents();
        DisableActieveElements();
    }

    private void OnPlayerDefeated()
    {
        _uiSystem.ShowDefeatedScreen();
        StopScenario();
    }
    private void OnEnemyDefeated()
    {
        _uiSystem.ShowYouWonScreen();
        StopScenario();
    }

    private void EnableActieveElements() => SwitchDisablableElementsEnabled(isEnabled: true);
    private void DisableActieveElements() => SwitchDisablableElementsEnabled(isEnabled: false);
    private void SwitchDisablableElementsEnabled(bool isEnabled)
    {
        for (int i = 0; i < _disablableElements.Length; i++)
        {
            if (_disablableElements[i] is IActivatable element)
            {
                if (isEnabled)
                    element.Enable();
                else
                    element.Disable();
            }
            else
            {
                _disablableElements[i] = null;
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