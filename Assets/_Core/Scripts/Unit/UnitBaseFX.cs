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
    //TODO: Consider global VXF pool for optimization if needed
    private Vector3 _unitViewPosition;
    private Quaternion _unitViewRotation;

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
        Tween.Rotation(_unitViewTransform, Vector3.up, duration: 0.5f);
        Tween.Position(_unitViewTransform, _unitViewTransform.localPosition - Vector3.up, duration: 0.5f);
        Tween.Delay(duration: 0.5f, () => onFXFinished?.Invoke());
    }

    public void ResetToNormal()
    {
        _unitMeshRenderer.material = _normalMaterial;            
        _unitViewTransform.localPosition = _unitViewPosition;
        _unitViewTransform.localRotation = _unitViewRotation;
    }
}
