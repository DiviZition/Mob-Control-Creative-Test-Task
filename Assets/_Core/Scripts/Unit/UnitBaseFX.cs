using PrimeTween;
using System;
using UnityEngine;

public class UnitBaseFX : MonoBehaviour
{
    [SerializeField] private Transform _unitViewTransform;
    [SerializeField] private ParticleSystem _spawnVFX;

    [SerializeField] private Material _normalMaterial;
    [SerializeField] private Material _deadMaterial;
    [SerializeField] private SkinnedMeshRenderer _unitMeshRenderer;
    [SerializeField] private Vector3 _dyingRotation;
    //TODO: Consider global VXF pool for optimization if needed
    private Vector3 _unitViewPosition;
    private Quaternion _unitViewRotation;
    private Sequence _deadFxSequence;

    private void Start()
    {
        _unitViewPosition = _unitViewTransform.localPosition;
        _unitViewRotation = _unitViewTransform.localRotation;
    }

    public void PlaySpawnVFX()
    {
        _spawnVFX.Play();
    }

    public void PlayDeadFx(Action onFXFinished)
    {
        _unitMeshRenderer.material = _deadMaterial;
        _deadFxSequence = Sequence.Create()
            .Group(Tween.Rotation(_unitViewTransform, _dyingRotation, duration: 0.7f))
            .Group(Tween.Position(_unitViewTransform, _unitViewTransform.position - Vector3.up * 0.2f, duration: 0.7f))
            .Group(Tween.Delay(duration: 0.5f, () => onFXFinished?.Invoke()));
    }

    public void ResetToNormal()
    {
        _deadFxSequence.Stop();
        _unitMeshRenderer.material = _normalMaterial;            
        _unitViewTransform.localPosition = _unitViewPosition;
        _unitViewTransform.localRotation = _unitViewRotation;
    }
}
