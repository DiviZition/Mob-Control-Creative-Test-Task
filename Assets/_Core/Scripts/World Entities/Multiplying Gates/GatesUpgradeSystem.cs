using System;

public class GatesUpgradeSystem : IDisposable
{
    private readonly IGateUpgradeView[] _gateUpgradeViews;
    private readonly MultiplyingGateSystem MultiplyingGateSystem;

    public GatesUpgradeSystem(MultiplyingGateSystem multiplyingGateSystem)
    {
        MultiplyingGateSystem = multiplyingGateSystem;

        foreach (var view in _gateUpgradeViews)
            view.OnUpgradePicked += CallForGatesUpgrade;
    }

    public void CallForGatesUpgrade(int upgradeValue, bool isMultiplying)
    {
        MultiplyingGateSystem.UpgradeAllGates(upgradeValue, isMultiplying);
    }

    public void Dispose()
    {
        foreach (var view in _gateUpgradeViews)
            view.OnUpgradePicked -= CallForGatesUpgrade;
    }
}