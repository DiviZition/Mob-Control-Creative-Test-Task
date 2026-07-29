using UnityEngine;
using UnityEngine.InputSystem;

public class CanonShooter : MonoBehaviour
{
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private Transform _unitSpawnPoint;
    [SerializeField] private float _shootThreashold = 0.1f;

    private float _nextTimeShotAvailable;

    public void Update()
    {
        if (Keyboard.current.spaceKey.isPressed == true && _nextTimeShotAvailable < Time.time)
        {
            _unitSpawner.SpawnUnit(_unitSpawnPoint.position, _unitSpawnPoint.localRotation);
            _nextTimeShotAvailable = Time.time + _shootThreashold;
        }
    }
}
