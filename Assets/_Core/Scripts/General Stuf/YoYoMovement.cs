using PrimeTween;
using UnityEngine;
using static UnityEditor.MaterialProperty;

public class PingPongMovement : MonoBehaviour
{
    [SerializeField] private Vector3 _leftPosition;
    [SerializeField] private Vector3 _rightPosition;
    [SerializeField] private float _duration;

    private Tween _tween;

    private void Start() => StartMovement();

    public void StartMovement()
    {
        // Если уже есть активный твины — остановим
        _tween.Stop();

        // Запускаем движение туда‑обратно
        _tween = Tween.Position
            (target: transform, 
            startValue: _leftPosition, 
            endValue: _rightPosition, 
            duration: _duration, 
            ease: Ease.InOutSine, 
            cycles: -1, 
            cycleMode: CycleMode.Yoyo);
    }

    public void StopMovement() => _tween.Stop();
    private void OnDestroy() => StopMovement();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_leftPosition, 0.2f);
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(_rightPosition, 0.2f);
    }
}
