using TMPro;
using UnityEngine;

public class HealthCounterView : MonoBehaviour
{
    [SerializeField] private TMP_Text _counterText;
    private IHealth _health;

    public void Init(IHealth health)
    {
        _health = health;
        _health.OnHealthChanged += UpdateTheCounter;
    }

    private void OnDestroy() => _health.OnHealthChanged -= UpdateTheCounter;

    public void UpdateTheCounter(int _ = 0, int currentHealth = 0) => _counterText.text = currentHealth.ToString();
}
