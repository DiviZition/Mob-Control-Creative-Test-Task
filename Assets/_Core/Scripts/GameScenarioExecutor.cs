using UnityEngine;
using UnityEngine.InputSystem;

public class GameScenarioExecutor : MonoBehaviour
{
    [SerializeField] private GameObject _scenario;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private float _cooldown = 0.1f;

    private float _nextSpawnAvailable;

    private void Update()
    {
        if (Keyboard.current.sKey.isPressed && _nextSpawnAvailable <= Time.time)
        {
            _unitSpawner.SpawnUnit(_spawnPosition.position, _spawnPosition.localRotation);
            _nextSpawnAvailable = Time.time + _cooldown;
        }
    }
}
