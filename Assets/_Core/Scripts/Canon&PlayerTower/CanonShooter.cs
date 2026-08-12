using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CanonShooter : ICanonShooter
{
    private UnitSpawner _unitSpawner;
    private ICanonMovement _canonMovement;
    private CanonSettingsConfig _config;
    private readonly IInputProvider _input;
    private float _shootCooldownTimer;

    public event Action OnCanonShoot;

    public CanonShooter(CanonSettingsConfig config, IInputProvider input, UnitSpawner unitSpawner, ICanonMovement canonMovement)
    {
        _config = config;
        _input = input;
        _unitSpawner = unitSpawner;
        _canonMovement = canonMovement;
    }

    public void UpdateLogic(float deltaTime)
    {
        _shootCooldownTimer -= deltaTime;
        if (_input.IsShooting && _shootCooldownTimer < 0)
        {
            ShootWithUnit();
            _shootCooldownTimer = _config.ShootThreashold;
            OnCanonShoot?.Invoke();
        }
    }

    public void ShootWithUnit()
    {
        Vector3 unitSpawnPosition = _canonMovement.CurrentPosition + _config.ShootPositionOffset;
        Quaternion unitSpawnRotation = Quaternion.Euler(unitSpawnPosition - _canonMovement.CurrentPosition);
        unitSpawnPosition += (Random.insideUnitSphere * Random.Range(-_config.ShootSpread, _config.ShootSpread)).ResetY();
        _unitSpawner.SpawnUnit(unitSpawnPosition, unitSpawnRotation);
    }
}