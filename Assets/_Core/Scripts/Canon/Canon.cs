using UnityEngine;

public class Canon : MonoBehaviour, IDisablable
{
    [SerializeField] private CanonShooter _shooter;
    [SerializeField] private CanonMovement _movement;

    private void Start() => Enable();

    public void Enable()
    {
        _shooter.enabled = true;
        _movement.enabled = true;
    }

    public void Disable()
    {
        _shooter.enabled = false;
        _movement.enabled = false;
    }
}
