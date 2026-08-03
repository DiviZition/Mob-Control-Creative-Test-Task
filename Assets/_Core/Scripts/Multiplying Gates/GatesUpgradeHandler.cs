using UnityEngine;

public class GatesUpgradeHandler : MonoBehaviour
{
    [SerializeField] private MultiplyingGate[] _gates;

    public void UpgradeAllGatesX(int upgradeValue, bool isMultiplying)
    {
        foreach (var gate in _gates)
        {
            if (isMultiplying)
                gate.IncreaseMultiplyingValue(gate.CurrentMultiplyingValue * upgradeValue - gate.CurrentMultiplyingValue);
            else
                gate.IncreaseMultiplyingValue(upgradeValue);
        }
    }
}
