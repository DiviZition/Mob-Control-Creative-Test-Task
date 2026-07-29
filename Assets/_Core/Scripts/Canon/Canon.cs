using UnityEngine;

public class Canon : MonoBehaviour
{
    [SerializeField] private CanonShooter _shooter;
    [SerializeField] private CanonMovement _movement;

    private void Start() => Activate();

    public void Activate()
    {
        _shooter.enabled = true;
        _movement.enabled = true;
    }
    public void DeActivate()
    {
        _shooter.enabled = true;
        _movement.enabled = true;
    }
}
