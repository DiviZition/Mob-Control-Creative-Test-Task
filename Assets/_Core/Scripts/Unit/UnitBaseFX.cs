using PrimeTween;
using R3;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitBaseFX : MonoBehaviour
{
    [SerializeField] private Transform _unitViewTransform;
    //TODO: Consider global VXF pool for optimization if needed
    [SerializeField] private ParticleSystem _spawnVFX;

    [Header("Sounds")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _deadSound;
    [SerializeField] private AudioClip _stepSound;
    [SerializeField] private float _stepSoundDelay;

    [Header("DeadFx settings")]
    [SerializeField] private Material _normalMaterial;
    [SerializeField] private Material _deadMaterial;
    [SerializeField] private SkinnedMeshRenderer _unitMeshRenderer;

    private Vector3 _unitViewPosition;
    private Quaternion _unitViewRotation;
    private Sequence _deadFxSequence;

    private IDisposable _stepSoundInvoker;

    private void Start()
    {
        _unitViewPosition = _unitViewTransform.localPosition;
        _unitViewRotation = _unitViewTransform.localRotation;
    }

    public void PlaySpawnVFX() => _spawnVFX.Play();

    public void PlayDeadFx(Action onFXFinished)
    {
        _stepSoundInvoker?.Dispose();
        _audioSource.Stop();
        _audioSource.spatialBlend = 0.5f;
        _audioSource.PlayOneShot(_deadSound);

        _unitMeshRenderer.material = _deadMaterial;
        _deadFxSequence = Sequence.Create()
            .Group(Tween.Rotation(_unitViewTransform, new Vector3(-60, 20, 45), duration: 0.7f))
            .Group(Tween.Position(_unitViewTransform, _unitViewTransform.position - Vector3.up * 0.2f, duration: 0.7f))
            .Group(Tween.Delay(duration: 0.5f, () => onFXFinished?.Invoke()));
    }

    public void ResetToNormal()
    {
        _audioSource.spatialBlend = 1f;
        _deadFxSequence.Stop();
        _unitMeshRenderer.material = _normalMaterial;            
        _unitViewTransform.localPosition = _unitViewPosition;
        _unitViewTransform.localRotation = _unitViewRotation;
    }

    private void PlayStepSound()
    {
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(_stepSound);
    }

    private void OnEnable()
    {
        _stepSoundInvoker = Observable.Interval(TimeSpan.FromSeconds(_stepSoundDelay)).Subscribe(_ => PlayStepSound());
    }

    private void OnDisable()
    {
        _stepSoundInvoker?.Dispose();
    }
}
