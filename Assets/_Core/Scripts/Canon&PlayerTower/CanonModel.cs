using System;

public class CanonModel : Lockable, ICanonModel
{
    public ICanonMovement Movement { get; private set; }
    public ICanonShooter Shooter { get; private set; }

    private IInputProvider _input;

    public CanonSettingsConfig Config { get; private set; }

    public CanonModel(IInputProvider input, CanonSettingsConfig config, UnitSpawner unitSpawner)
    {
        _input = input;
        Config = config;

        Movement = new CanonMovement(config, _input);
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
