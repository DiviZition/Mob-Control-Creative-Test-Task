using UnityEngine;

public class MultiplyingGateSystemViews : MonoBehaviour
{
    [SerializeField] private MultiplyingGateView[] _gatesViews;
    [SerializeField] private GateUpgradeView[] _gatesUpgrades;

    private IMultiplyingGateSystem _gatesSystem;

    public void Init(IMultiplyingGateSystem gatesSystem)
    {
        _gatesSystem = gatesSystem;

        for (int i = 0; i < _gatesViews.Length; i++)
        {
            _gatesSystem.RegisterMultiplyingGate(i, _gatesViews[i].InitialMultiplyer);
            _gatesViews[i].OnUnitEntered += _gatesSystem.MultiplyUnits;
        }

        foreach (var upgrade in _gatesUpgrades)
            upgrade.OnUpgradePicked += _gatesSystem.UpgradeAllGates;
    }

    private void OnDestroy()
    {
        foreach (var gate in _gatesViews)
            gate.OnUnitEntered -= _gatesSystem.MultiplyUnits;

        foreach (var upgrade in _gatesUpgrades)
            upgrade.OnUpgradePicked -= _gatesSystem.UpgradeAllGates;
    }
}
