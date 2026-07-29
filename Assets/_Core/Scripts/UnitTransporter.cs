using MoreMountains.Feedbacks;
using PrimeTween;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[SelectionBase]
public class UnitTransporter : MonoBehaviour
{
    [SerializeField] private Transform _enterPosition;
    [SerializeField] private Transform _exitPosition;

    [SerializeField] private MMF_Player _tubeEnterFeedback;
    [SerializeField] private MMF_Player _tubeExitFeedback;

    [SerializeField] private double _transportTime = 1.5f;
    [SerializeField] private UnitBattleSide _whoToSweep;

    private Queue<ScheduledUnitTracker> _unitsInside = new Queue<ScheduledUnitTracker>(256);
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private IDisposable _timer;

    private const float CheckInterval = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UnitBase unit) && unit.UnitBattleSide == _whoToSweep)
            PlaceUnitInTube(unit);
    }

    public void PlaceUnitInTube(UnitBase unit)
    {
        unit.Disable();
        Tween.Position(unit.Transform, _enterPosition.position, duration: 0.5f, ease: Ease.OutCubic);
        Tween.Delay(duration: 0.3f, () => unit.gameObject.SetActive(false));
        Tween.Delay(duration: 0.5f, () => ScheduleUnitsOutput(unit));

        _tubeEnterFeedback.PlayFeedbacks();
    }

    private void ScheduleUnitsOutput(UnitBase unit)
    {
        _unitsInside.Enqueue(new ScheduledUnitTracker(unit, Time.time));

        if (_timer == null)
        {
            _timer = Observable
                .Interval(TimeSpan.FromSeconds(CheckInterval), _cts.Token)
                .Subscribe(_ => OutputAllReadyUnits());
        }
    }

    private void OutputAllReadyUnits()
    {
        while (_unitsInside.Count > 0 && Time.time - _unitsInside.Peek().TubeEnterTime > _transportTime)
            OutputUnit(_unitsInside.Dequeue().Unit);

        if (_unitsInside.Count <= 0)
        {
            _timer?.Dispose();
            _timer = null;
        }
    }

    private void OutputUnit(UnitBase unitToOutput)
    {
        _tubeExitFeedback.PlayFeedbacks();
        unitToOutput.Movement.WarpAgent(_exitPosition.position);
        unitToOutput.Movement.RotateUnit(_exitPosition.rotation);
        unitToOutput.gameObject.SetActive(true);
        unitToOutput.Enable();
    }

    private void OnDestroy()
    {
        _timer?.Dispose();
    }

    struct ScheduledUnitTracker
    {
        public UnitBase Unit { get; private set; }
        public float TubeEnterTime { get; private set; }

        public ScheduledUnitTracker(UnitBase unit, float enterTime)
        {
            Unit = unit;
            TubeEnterTime = enterTime;
        }
    }
}
