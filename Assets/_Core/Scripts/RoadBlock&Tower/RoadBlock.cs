using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RoadBlock : MonoBehaviour, IDamageable
{
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public UnitBattleSide BattleSide { get; private set; }

    [SerializeField] private TMP_Text _counterText;

    public event Action OnDead;

    public int CurrentHealth {  get; private set; }
    public bool ReturnsDamage => true;

    protected virtual void Start()
    {
        CurrentHealth = MaxHealth;
        UpdateCounter();
    }

    private void UpdateCounter() => _counterText.text = CurrentHealth.ToString();

    public void TakeDamage(int damage)
    {
        if (CurrentHealth - damage < 0)
        {
            CurrentHealth = 0;
            RemoveRoadBlock();
        }
        else
        {
            CurrentHealth -= damage;
        }

        UpdateCounter();
    }

    protected virtual void RemoveRoadBlock()
    {
        OnDead?.Invoke();
        Destroy(this.gameObject);
    }
}
