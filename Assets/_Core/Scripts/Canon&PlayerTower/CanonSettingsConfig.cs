using UnityEngine;

[CreateAssetMenu(fileName = "CanonConfig", menuName = "Configs/CanonSettings")]
public class CanonSettingsConfig : ScriptableObject
{
    [SerializeField] public float MoveSpeed = 4;
    [SerializeField] public float WheelsRotationSpeed = 1700;
    [SerializeField] public float MoveAcceleration = 7;
    [SerializeField] public float LocalMoveBounds = 2.35f;
    [SerializeField] public float ShootThreashold = 0.1f;
    [SerializeField] public float ShootSpread = 0.01f;
    [SerializeField] public Vector3 ShootPositionOffset = Vector3.forward;
}