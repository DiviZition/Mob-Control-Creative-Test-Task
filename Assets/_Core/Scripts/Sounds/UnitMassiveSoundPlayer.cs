using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitMassiveSoundPlayer : MonoBehaviour, IActivatable
{
    [SerializeField] private UnitSpawner _spawnerToObserve;
    [SerializeField] private float _throttleSoundsWindow;

    [SerializeField] private RevolverSoundPool _stepSoundPool;
    [SerializeField] private RevolverSoundPool _dieSoundPool;

    private IDisposable _stepSoundSub;
    private IDisposable _dieSoundSubs;
    private Dictionary<UnitBase, Action> _dieSoundsSubCollection = new Dictionary<UnitBase, Action>(1024);

    private void Start() => Enable();

    public void PlayDieSoundOnUnit(Transform unitsTransform) => _dieSoundPool.PlaySoundOnUnit(unitsTransform);
    public void PlayStepSoundOnUnit(Transform unitsTransform) => _stepSoundPool.PlaySoundOnUnit(unitsTransform);

    public void Enable()
    {
        _stepSoundPool.Enable();
        _dieSoundPool.Enable();

        _stepSoundSub = _spawnerToObserve.OnUnitSpawned
            .ThrottleFirst(TimeSpan.FromSeconds(_throttleSoundsWindow))
            .Subscribe(unit => PlayStepSoundOnUnit(unit.Transform));

        var dieSoundSub = _spawnerToObserve.OnUnitSpawned.Subscribe(unit =>
        {
            if (_dieSoundsSubCollection.ContainsKey(unit) == false)
            {
                Action onDead = () => PlayDieSoundOnUnit(unit.Transform);
                unit.OnDead += onDead;
                _dieSoundsSubCollection.Add(unit, onDead);
            }
        });

        var dieSoundUnsub = _spawnerToObserve.OnUnitDespawned.Subscribe(unit => unit.OnDead -= _dieSoundsSubCollection[unit]);

        _dieSoundSubs = Disposable.Combine(dieSoundSub, dieSoundUnsub);
    }

    public void Disable()
    {
        _stepSoundPool.Disable();
        _dieSoundPool.Disable();

        _stepSoundSub?.Dispose();
        _dieSoundSubs?.Dispose();
        foreach (var sub in _dieSoundsSubCollection)
            sub.Key.OnDead -= sub.Value;
    }
}


[Serializable]
public struct RevolverSoundPool : IActivatable
{
    [SerializeField] private AudioSource _audiosSourceReference;
    [SerializeField] private Transform _soundSourcesContainer;
    [SerializeField] private Transform _disabledSpawnContainer;

    [SerializeField] private int _maxSoundsPerType;

    private Queue<AudioSourceWrapper> _soundsPool;

    public void PlaySoundOnUnit(Transform unitsTransform)
    {
        var source = _soundsPool.Dequeue();
        _soundsPool.Enqueue(source);
        source.Transform.gameObject.SetActive(false);
        source.Transform.SetParent(unitsTransform);
        source.Transform.localPosition = Vector3.zero;
        source.Transform.gameObject.SetActive(true);
        source.Source.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        source.Source.Play();
    }

    public void Enable()
    {
        if (_soundsPool == null)
        {
            _soundsPool = new Queue<AudioSourceWrapper>(_maxSoundsPerType);
            for (int i = 0; i < _maxSoundsPerType; i++)
            {
                AudioSource newSource = MonoBehaviour.Instantiate(_audiosSourceReference, _disabledSpawnContainer);
                newSource.gameObject.SetActive(false);

                Transform sourceTransform = newSource.transform;
                sourceTransform.SetParent(_soundSourcesContainer);
                _soundsPool.Enqueue(new AudioSourceWrapper(newSource, sourceTransform));
            }
        }
    }

    public void Disable()
    {
        foreach (var sourceWrappers in _soundsPool)
        {
            sourceWrappers.Transform.gameObject.SetActive(false);
            sourceWrappers.Transform.SetParent(_soundSourcesContainer);
        }
    }

    private struct AudioSourceWrapper
    {
        public AudioSource Source { get; private set; }
        public Transform Transform { get; private set; }

        public AudioSourceWrapper(AudioSource source, Transform transform)
        {
            Source = source;
            Transform = transform;
        }
    }
}