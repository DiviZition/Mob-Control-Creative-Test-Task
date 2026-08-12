using UnityEngine;

public class CanonMovement : ICanonMovement
{
    private CanonSettingsConfig _config;
    private readonly IInputProvider _input;
    private float _currentDirectionAccelerated;
    private Vector3 _initialPosition;
    private Vector2 _localMoveBounds;
    public Vector3 CurrentPosition { get; private set; }

    public CanonMovement(CanonSettingsConfig config, IInputProvider input, Vector3 initialPosition = default)
    {
        _config = config;
        _input = input;
        _initialPosition = initialPosition;
        CurrentPosition = _initialPosition;
        _localMoveBounds = new Vector2(_initialPosition.x - _config.LocalMoveBounds, _initialPosition.x + _config.LocalMoveBounds);
    }

    public void UpdateLogic(float deltaTime)
    {
        if (_input.AxisHorizontal != 0 || _currentDirectionAccelerated == 0)
            GetNextCanonPosition(_input.AxisHorizontal, deltaTime);
    }

    private void GetNextCanonPosition(float xDirection, float deltaTime)
    {
        if (xDirection == 0 && _currentDirectionAccelerated == 0)
            return;

        _currentDirectionAccelerated = Mathf.MoveTowards(_currentDirectionAccelerated, xDirection, _config.MoveAcceleration * deltaTime);

        Vector3 moveDelta = Vector3.right * _currentDirectionAccelerated * _config.MoveSpeed * deltaTime;
        if (CheckIfInBounds(CurrentPosition + moveDelta))
        {
            _currentDirectionAccelerated = 0;
            return;
        }

        CurrentPosition += moveDelta;
    }

    private bool CheckIfInBounds(Vector3 position) => position.x > _localMoveBounds.x && position.x < _localMoveBounds.y;
}