using UnityEngine;

public class RoadBlock : Damageable
{
    protected override void PerformDeath()
    {
        base.PerformDeath();
        Destroy(gameObject);
    }
}
