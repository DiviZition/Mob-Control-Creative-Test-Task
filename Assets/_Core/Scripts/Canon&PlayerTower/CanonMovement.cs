using UnityEngine;

public interface ICanonMovement : IUpdatable
{
    public Vector3 CurrentPosition { get; }
}

public class CanonMovement : ICanonMovement
{
    private CanonSettingsConfig _config;
    private readonly IInputProvider _input;
    private float _currentDirectionAccelerated;
    private Vector3 _initialPosition;
    private Vector2 _localMoveBounds;
    public Vector3 CurrentPosition { get; private set; }

    public CanonMovement(CanonSettingsConfig config, IInputProvider input, Vector3 initialPosition)
    {
        _config = config;
        _input = input;
        _initialPosition = initialPosition;
        CurrentPosition = _initialPosition;
        _localMoveBounds = new Vector2(CurrentPosition.x - _config.LocalMoveBounds, CurrentPosition.x + _config.LocalMoveBounds);
    }

    public void UpdateLogic(float deltaTime) => GetNextCanonPosition(_input.AxisHorizontal, deltaTime);

    private void GetNextCanonPosition(float xDirection, float deltaTime)
    {
        if (xDirection == 0 && _currentDirectionAccelerated == 0)
            return;

        _currentDirectionAccelerated = Mathf.MoveTowards(_currentDirectionAccelerated, xDirection, _config.MoveAcceleration * deltaTime);

        Vector3 moveDelta = Vector3.right * _currentDirectionAccelerated * _config.MoveSpeed * deltaTime;
        if (CheckIfInBounds(CurrentPosition + moveDelta) == false)
        {
            _currentDirectionAccelerated = 0;
            return;
        }

        CurrentPosition += moveDelta;
    }

    private bool CheckIfInBounds(Vector3 position) => position.x > _localMoveBounds.x && position.x<_localMoveBounds.y;
}