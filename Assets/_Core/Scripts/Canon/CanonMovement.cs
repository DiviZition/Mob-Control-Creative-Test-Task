using UnityEngine;
using UnityEngine.InputSystem;

public class CanonMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _wheelsRotationSpeed;
    [SerializeField] private float _moveAcceleration;

    [SerializeField] private float _moveBounds;

    [SerializeField] private Transform _canonTransform;
    [SerializeField] private Transform[] _wheels;

    private float _currentDirectionAccelerated;

    public void Update()
    {
        float xInputDirection = 0;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            xInputDirection = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            xInputDirection = 1;

        MoveCanon(xInputDirection);
    }

    private void MoveCanon(float xDirection)
    {
        _currentDirectionAccelerated = Mathf.MoveTowards(_currentDirectionAccelerated, xDirection, _moveAcceleration * Time.deltaTime);

        if (_currentDirectionAccelerated == 0)
            return;

        Vector3 moveDelta = Vector3.right * _currentDirectionAccelerated * _moveSpeed * Time.deltaTime;
        if (Mathf.Abs((_canonTransform.localPosition + moveDelta).x) >= _moveBounds)
        {
            _currentDirectionAccelerated = 0;
            return;
        }

        _canonTransform.localPosition += moveDelta;
        foreach (Transform wheel in _wheels)
            wheel.Rotate(Vector3.forward, _wheelsRotationSpeed * Time.deltaTime * (_currentDirectionAccelerated * -1));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Vector3 rayStartPosition = _canonTransform.position.ResetX(_moveBounds * -1);
        Gizmos.DrawRay(rayStartPosition, Vector3.right * _moveBounds * 2);
    }
}