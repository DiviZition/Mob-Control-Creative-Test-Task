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

        _gatesSystem.OnGatesUpgraded += OnGateUpgraded;
    }

    public void OnGateUpgraded(int gateID, int newValue) => _gatesViews[gateID].OnGateUpgrade(newValue);

    private void OnDestroy()
    {
        if (_gatesSystem == null) return;

        foreach (var gate in _gatesViews)
            gate.OnUnitEntered -= _gatesSystem.MultiplyUnits;

        foreach (var upgrade in _gatesUpgrades)
            upgrade.OnUpgradePicked -= _gatesSystem.UpgradeAllGates;

        _gatesSystem.OnGatesUpgraded -= OnGateUpgraded;
    }
}
