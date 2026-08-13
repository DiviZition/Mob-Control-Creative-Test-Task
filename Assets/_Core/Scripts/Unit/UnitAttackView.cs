using System;
using UnityEngine;

public class UnitAttackView : MonoBehaviour
{
    private IUnitAttack _attackModel;

    public void Init(IUnitAttack attackModel) => _attackModel = attackModel;

    private void OnTriggerEnter(Collider other) => _attackModel.RegisterNewPossibleTarget(other);
    private void OnTriggerExit(Collider other) => _attackModel.RemovePossibleTarget(other);
}
