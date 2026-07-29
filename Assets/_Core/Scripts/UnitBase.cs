using UnityEngine;

public class UnitBase : MonoBehaviour, IPoolable
{
    [field: SerializeField]public Transform Transform { get; private set; }

    public void ActivatePoolable()
    {
        Debug.Log($"Activating unit: [{this.name}]");
    }

    public void DeactivatePoolable()
    {
        Debug.Log($"Deactivating unit: [{this.name}]");
    }
}