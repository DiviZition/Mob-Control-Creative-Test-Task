using UnityEngine;

public class BossUnitAttacker : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _attackCoolDown;
    [SerializeField] private UnitBase _unitBase;

    private float _nextAttackAvailable;


}
