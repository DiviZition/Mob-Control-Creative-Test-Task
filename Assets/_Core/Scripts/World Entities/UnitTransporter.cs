using MoreMountains.Feedbacks;
using PrimeTween;
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

    [SerializeField] private float _transportTime = 1.5f;
    [SerializeField] private float _nextTickTime;
    [SerializeField] private UnitBattleSide _whoToSweep;

    private Queue<ScheduledUnitTracker> _unitsInside = new Queue<ScheduledUnitTracker>(256);

    private const float CheckInterval = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IUnitView unit) && unit.BattleSide == _whoToSweep)
            PlaceUnitInTube(unit);
    }

    private void Update()
    {
        if (_nextTickTime > Time.time) return;

        _nextTickTime = Time.time + CheckInterval;
        if (_unitsInside.Count > 0)
            OutputAllReadyUnits();
    }

    public void PlaceUnitInTube(IUnitView unit)
    {
        unit.Movement_Lock();

        Tween.Position(unit.Transform, _enterPosition.position, duration: 0.5f, ease: Ease.OutCubic);
        Tween.Delay(duration: 0.3f, () => unit.SetViewEnabled(false));
        Tween.Delay(duration: 0.5f, () => ScheduleUnitsOutput(unit));

        _tubeEnterFeedback.PlayFeedbacks();
    }

    private void ScheduleUnitsOutput(IUnitView unit) => _unitsInside.Enqueue(new ScheduledUnitTracker(unit, Time.time));

    private void OutputAllReadyUnits()
    {
        while (_unitsInside.Count > 0 && Time.time - _unitsInside.Peek().TubeEnterTime > _transportTime)
            OutputUnit(_unitsInside.Dequeue().Unit);
    }

    private void OutputUnit(IUnitView unitView)
    {
        _tubeExitFeedback.PlayFeedbacks();

        unitView.Movement_SetPosition(_exitPosition.position);
        unitView.Movement_SetDirection(_exitPosition.rotation);
        unitView.Movement_UnLock();

        unitView.SetViewEnabled(true);
    }

    struct ScheduledUnitTracker
    {
        public IUnitView Unit { get; private set; }
        public float TubeEnterTime { get; private set; }

        public ScheduledUnitTracker(IUnitView unit, float enterTime)
        {
            Unit = unit;
            TubeEnterTime = enterTime;
        }
    }
}
