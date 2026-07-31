using TMPro;
using UnityEngine;

public class HealthCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text _counterText;
    [SerializeField] private Damageable _damageable;

    public void UpdateTheCounter() => _counterText.text = _damageable.CurrentHealth.ToString();

    private void OnEnable()
    {
        _damageable.OnHealthRestored += UpdateTheCounter;
        _damageable.OnTakeDamage += UpdateTheCounter;
    }

    private void OnDisable()
    {
        _damageable.OnHealthRestored -= UpdateTheCounter;
        _damageable.OnTakeDamage -= UpdateTheCounter;
    }
}
