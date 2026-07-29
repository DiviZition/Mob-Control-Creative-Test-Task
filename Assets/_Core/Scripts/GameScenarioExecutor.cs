using UnityEngine;
using UnityEngine.InputSystem;

public class GameScenarioExecutor : MonoBehaviour
{
    [SerializeField] private GameObject _scenario;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private UnitSpawner _unitSpawner;
}
