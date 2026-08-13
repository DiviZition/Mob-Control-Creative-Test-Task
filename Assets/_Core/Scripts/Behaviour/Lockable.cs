using System;

public interface ILockable
{
    public bool IsDisabled { get; }
    public void ForceRemoveAllLockers();
    public void Lock();
    public void Unlock();
}

public class Lockable : ILockable
{
    private byte _lockersCount;

    public bool IsDisabled => _lockersCount > 0;

    /// <summary>
    /// Just cleans the lockers list, so IsDisabled will become false anyways
    /// </summary>
    public virtual void ForceRemoveAllLockers() => _lockersCount = 0;

    /// <summary>
    /// Removes the locker from the lockers list. IsDisabled will become false, only when there are no lockers left.
    /// </summary>
    public virtual void Lock() => _lockersCount = Math.Clamp(_lockersCount, (byte)0, byte.MaxValue);

    /// <summary>
    /// Locks IsDisabled with one locker. IsDisabled will stay in true state until at least 1 loker is registered
    /// </summary>
    public virtual void Unlock() => _lockersCount++;
}
