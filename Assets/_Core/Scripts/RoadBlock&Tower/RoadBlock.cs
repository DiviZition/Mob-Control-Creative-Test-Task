using UnityEngine;

public class RoadBlock : Damageable, IDisablable
{
    protected override void PerformDeath()
    {
        base.PerformDeath();
        Disable();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        RestoreHealth();
        gameObject.SetActive(true);
    }
}
