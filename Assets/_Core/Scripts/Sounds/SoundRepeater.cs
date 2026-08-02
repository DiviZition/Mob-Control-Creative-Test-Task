using R3;
using System;
using UnityEngine;

public class SoundRepeater : MonoBehaviour, IActivatable
{
    [SerializeField] private bool _enabledFromStart;

    public AudioSource SourceToRepeat;
    public float RepeatDelay;

    private IDisposable _repeatSub;

    public void Enable()
    {
        _repeatSub = Observable
            .Interval(TimeSpan.FromSeconds(SourceToRepeat.clip.length + RepeatDelay))
            .Subscribe(_ =>
            {
                if (SourceToRepeat.enabled)
                {
                    SourceToRepeat.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                    SourceToRepeat.Play();
                }
            });
    }

    public void Disable()
    {
        _repeatSub?.Dispose();
    }

    private void OnEnable()
    {
        if (_enabledFromStart)
            Enable();
    }
    private void OnDisable() => Disable(); 
}
