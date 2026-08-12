using TMPro;
using UnityEngine;

public class HealthCounterView : MonoBehaviour
{
    [SerializeField] private TMP_Text _counterText;
    [SerializeField] private DamageableView _damageable;

    public void UpdateTheCounter(int currentHealth) => _counterText.text = currentHealth.ToString();
}
