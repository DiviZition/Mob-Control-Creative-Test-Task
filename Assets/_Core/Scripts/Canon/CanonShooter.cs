using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanonShooter : MonoBehaviour
{
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private MMF_Player _shootEffect;
    [SerializeField] private Transform _unitSpawnPoint;
    [SerializeField] private float _shootThreashold = 0.1f;
    [SerializeField] private float _shootOffset = 0.01f;

    private float _nextTimeShotAvailable;

    public void Update()
    {
        if (Keyboard.current.spaceKey.isPressed == true && _nextTimeShotAvailable < Time.time)
        {
            _shootEffect.ResetFeedbacks();
            _shootEffect.RestoreInitialValues();
            _shootEffect.PlayFeedbacks();
            Vector3 unitSpawnPosition = _unitSpawnPoint.position + (_unitSpawnPoint.right * Random.Range(-_shootOffset, _shootOffset));
            _unitSpawner.SpawnUnit(unitSpawnPosition, _unitSpawnPoint.localRotation);
            _nextTimeShotAvailable = Time.time + _shootThreashold;
        }
    }
}
