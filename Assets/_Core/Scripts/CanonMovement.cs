using MoreMountains.Feedbacks;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CanonView : MonoBehaviour
{
    [field: SerializeField] public Transform CanonTransform {  get; private set; }
    [field: SerializeField] public Transform UnitSpawnPointer {  get; private set; }
    
    [field: SerializeField] public CanonSettingsConfig CanonConfig {  get; private set; }
    [field: SerializeField] public UnitConfig PlayerUnitConfig {  get; private set; }

    [field: SerializeField] public MMF_Player CanonHitFeedback {  get; private set; }
    [field: SerializeField] public MMF_Player ShootEffect {  get; private set; }

    public float GizmosLocalMoveBounds = 0;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Vector3 rayStartPosition = CanonTransform.localPosition.ResetX(GizmosLocalMoveBounds * -1);
        Gizmos.DrawRay(rayStartPosition, Vector3.right * GizmosLocalMoveBounds * 2);
    }
}

public class CanonPresenter
{
    private CanonView _view;

    public CanonPresenter(CanonView canonView, CanonSettingsConfig config)
    {
        _view = canonView;

        _view.GizmosLocalMoveBounds = config.LocalMoveBounds;
    }

    public Vector3 GetUnitSpawnPosition() => _view.UnitSpawnPointer.position;
    public Quaternion GetUnitSpawnDirection() => _view.UnitSpawnPointer.rotation;
    public void SetCanonPosition(Vector3 newPosition) => _view.CanonTransform.localPosition = newPosition;

    public void PlayShotEffect()
    {
        _view.ShootEffect.ResetFeedbacks();
        _view.ShootEffect.RestoreInitialValues();
        _view.ShootEffect.PlayFeedbacks();
    }

    public void PlayKickEffect()
    {
        _view.CanonHitFeedback.ResetFeedbacks();
        _view.CanonHitFeedback.RestoreInitialValues();
        _view.CanonHitFeedback.PlayFeedbacks();
    }
}

public class CanonModel : IUpdatable
{
    private CanonMovement _movement;
    private CanonShooter _shooter;
    private CanonPresenter _presenter;

    private IInputProvider _input;

    public bool IsDisabled { get; private set; }

    public CanonModel(IInputProvider input, CanonPresenter presenter, CanonSettingsConfig config, UnitSpawner unitSpawner)
    {
        _input = input;
        IsDisabled = false;

        _presenter = presenter;

        _movement = new CanonMovement(config);
        _shooter = new CanonShooter(config, unitSpawner, _presenter);
    }

    public void UpdateLogic(float deltaTime)
    {
        if (IsDisabled == true)
            return;

        var nextCanonPosition = _movement.GetNextCanonPosition(xDirection: _input.AxisHorizontal, deltaTime: deltaTime);
        _presenter.SetCanonPosition(nextCanonPosition);

        if (_input.IsShooting)
            if (_shooter.TryShootWithUnit())
                _presenter.PlayShotEffect();
    }
}

public class CanonMovement
{
    private CanonSettingsConfig _config;

    private float _currentDirectionAccelerated;
    private Vector3 _initialPosition;
    private Vector3 _currentPosition;

    public CanonMovement(CanonSettingsConfig config)
    {
        _config = config;
        _initialPosition = Vector3.zero;
        _currentPosition = _initialPosition;
    }

    public Vector3 GetNextCanonPosition(float xDirection, float deltaTime)
    {
        if (xDirection == 0 && _currentDirectionAccelerated == 0)
            return _currentPosition;

        _currentDirectionAccelerated = Mathf.MoveTowards(_currentDirectionAccelerated, xDirection, _config.MoveAcceleration * deltaTime);

        Vector3 moveDelta = Vector3.right * _currentDirectionAccelerated * _config.MoveSpeed * deltaTime;
        if (Mathf.Abs((_currentPosition + moveDelta).x) >= _config.LocalMoveBounds)
        {
            _currentDirectionAccelerated = 0;
            return _currentPosition;
        }

        _currentPosition += moveDelta;
        return _currentPosition;
    }
}

public class CanonShooter
{
    private UnitSpawner _unitSpawner;
    private CanonPresenter _canonPresenter;
    private CanonSettingsConfig _config;

    private float _nextTimeShotAvailable;

    public CanonShooter(CanonSettingsConfig config, UnitSpawner unitSpawner, CanonPresenter canonPresenter)
    {
        _config = config;
        _unitSpawner = unitSpawner;
        _canonPresenter = canonPresenter;
    }

    public bool TryShootWithUnit()
    {
        if (_nextTimeShotAvailable > Time.time)
            return false;

        Vector3 unitSpawnPosition = _canonPresenter.GetUnitSpawnPosition();
        unitSpawnPosition += (Random.insideUnitSphere * Random.Range(-_config._shootSpread, _config._shootSpread)).ResetY();
        _unitSpawner.SpawnUnit(unitSpawnPosition, _canonPresenter.GetUnitSpawnDirection());
        _nextTimeShotAvailable = Time.time + _config._shootThreashold;
        return true;
    }
}