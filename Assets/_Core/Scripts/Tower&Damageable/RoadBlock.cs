using UnityEngine;

public class RoadBlock : Damageable, IActivatable
{
    private void Start() => Enable();

    protected override void PerformDeath()
    {
        base.PerformDeath();
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        RestoreHealth();
        gameObject.SetActive(true);
    }

    public void Disable() { }
}
