using UnityEngine;

public class GatesUpgradeHandler : MonoBehaviour
{
    [SerializeField] private MultiplyingGate[] _gates;

    public void UpgradeAllGatesX(int additionalX)
    {
        foreach (var gate in _gates)
        {
            gate.IncreaseMultiplyingValue(additionalX);
        }
    }
}
