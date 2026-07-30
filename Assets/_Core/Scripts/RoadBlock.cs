using System;
using TMPro;
using UnityEngine;



public class RoadBlock : MonoBehaviour, IDamageable
{
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public UnitBattleSide BattleSide { get; private set; }

    [SerializeField] private TMP_Text _counterText;

    public int CurrentHealth {  get; private set; }

    private void Start()
    {
        CurrentHealth = MaxHealth;
        UpdateCounter();
    }

    private void UpdateCounter() => _counterText.text = CurrentHealth.ToString();

    public void TakeDamage(int damage, IDamageable damageDealler)
    {
        if (CurrentHealth - damage < 0)
        {
            CurrentHealth = 0;
            RemoveRoadBlock();
        }
        else
        {
            CurrentHealth -= damage;
            damageDealler.TakeDamage(damage, this);
        }

        UpdateCounter();
    }

    private void RemoveRoadBlock()
    {
        Destroy(this.gameObject);
    }
}
