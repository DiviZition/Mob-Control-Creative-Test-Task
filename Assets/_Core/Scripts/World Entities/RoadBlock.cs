using UnityEngine;

public class RoadBlock : DamageableView, IActivatable
{
    private void Start() => Enable();

    public void PerformDeath()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable() { }
}
