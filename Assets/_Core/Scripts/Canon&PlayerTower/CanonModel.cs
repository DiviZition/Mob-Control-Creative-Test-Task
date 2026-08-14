using System;
using UnityEngine;

public interface ICanonModel : IUpdatable, ILockable
{
    public ICanonMovement Movement { get; }
    public ICanonShooter Shooter { get; }
    public CanonSettingsConfig Config { get; }
}


public class CanonModel : Lockable, ICanonModel
{
    public ICanonMovement Movement { get; private set; }
    public ICanonShooter Shooter { get; private set; }

    private IInputProvider _input;

    public CanonSettingsConfig Config { get; private set; }

    public CanonModel(IInputProvider input, CanonSettingsConfig config, UnitSpawner unitSpawner, Vector3 canonInitialPosition)
    {
        _input = input;
        Config = config;

        Movement = new CanonMovement(config, _input, canonInitialPosition);
        Shooter = new CanonShooter(config, _input, unitSpawner, Movement);
    }

    public void UpdateLogic(float deltaTime)
    {
        if (IsDisabled == true)
            return;

        Movement.UpdateLogic(deltaTime);
        Shooter.UpdateLogic(deltaTime);
    }
}
