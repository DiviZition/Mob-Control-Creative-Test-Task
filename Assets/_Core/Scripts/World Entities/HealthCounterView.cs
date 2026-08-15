using TMPro;
using UnityEngine;

public class HealthCounterView : MonoBehaviour
{
    [SerializeField] private TMP_Text _counterText;
    private Health _health;

    public void Init(Health health)
    {
        _health = health;
        _health.OnHealthChanged += CounterUpdateRetranslate;
        UpdateTheCounter(_health.MaxHealth);
    }

    private void OnDestroy()
    {
        if (_health != null)
            _health.OnHealthChanged -= CounterUpdateRetranslate;
    }

    public void CounterUpdateRetranslate(int maxHealth, int currentHealth) => UpdateTheCounter(currentHealth);
    public void UpdateTheCounter(int currentHealth = 0) => _counterText.text = currentHealth.ToString();
}
