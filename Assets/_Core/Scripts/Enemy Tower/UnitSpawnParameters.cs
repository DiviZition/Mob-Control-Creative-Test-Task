using UnityEngine;

public class UnitSpawnParameters
{
    public Vector3 UnitSpawnPosition { get; private set; }
    public Quaternion UnitInitialDirection { get; private set; }
    public float SpawnPositionOffset { get; private set; }
    public Transform UnitsContainer { get; private set; }

    public UnitSpawnParameters() { }
    public UnitSpawnParameters(Transform reference, float spawnPositionOffset, Transform unitContainer)
        : this(reference.position, reference.rotation, spawnPositionOffset, unitContainer) { }
    public UnitSpawnParameters(Vector3 unitSpawnPosition, Quaternion unitInitialDirection, float spawnPositionOffset, Transform unitContainer)
    {
        UnitSpawnPosition = unitSpawnPosition;
        UnitInitialDirection = unitInitialDirection;
        SpawnPositionOffset = spawnPositionOffset;
        UnitsContainer = unitContainer;
    }

    public static UnitSpawnParameters Create() => new UnitSpawnParameters();
    public UnitSpawnParameters WithSpawnPosition(Vector3 spawnPosition)
    {
        UnitSpawnPosition = spawnPosition;
        return this;
    }
    public UnitSpawnParameters WithInitialRotation(Quaternion initialRotation)
    {
        UnitInitialDirection = initialRotation;
        return this;
    }
    public UnitSpawnParameters WithSpawnPositionOffset(float spawnPositionOffset)
    {
        SpawnPositionOffset = spawnPositionOffset;
        return this;
    }
    public UnitSpawnParameters WithContainer(Transform unitContainer)
    {
        UnitsContainer = unitContainer;
        return this;
    }
}
