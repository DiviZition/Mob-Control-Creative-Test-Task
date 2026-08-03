using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [SerializeField] private Transform _objectToRotate;
    [SerializeField] private Vector3 _rotationDirection;
    [SerializeField] private float _rotationForce;

    private Quaternion _initialRotation;

    private void OnValidate()
    {
        if (_objectToRotate == null)
            _objectToRotate = this.transform;
    }

    private void Start()
    {
        _initialRotation = transform.localRotation;
    }

    private void Update()
    {
        float angleRotatingTo = (Time.time * _rotationForce) % 360;
        _objectToRotate.localRotation = _initialRotation * Quaternion.AngleAxis(angleRotatingTo, _rotationDirection);
    }
}
