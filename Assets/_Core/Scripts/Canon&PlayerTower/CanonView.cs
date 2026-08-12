using MoreMountains.Feedbacks;
using UnityEngine;

public class CanonView : MonoBehaviour
{
    [field: SerializeField] public Transform Transform {  get; private set; }
    [field: SerializeField] public CanonSettingsConfig CanonConfig {  get; private set; }
    [field: SerializeField] public UnitConfig PlayerUnitConfig {  get; private set; }

    [field: SerializeField] public MMF_Player ShootEffect {  get; private set; }

    [SerializeField] private CanonSettingsConfig _config;

    public ICanonModel _canonModel;

    public void Init(ICanonModel canonModel)
    {
        _canonModel = canonModel;
        _canonModel.Shooter.OnCanonShoot += PlayShootEffect;
    }

    private void OnDestroy() => _canonModel.Shooter.OnCanonShoot += PlayShootEffect;

    private void Update()
    {
        if (_canonModel == null || _canonModel.IsDisabled) return;

        Transform.localPosition = _canonModel.Movement.CurrentPosition;
    }

    public void PlayShootEffect()
    {
        ShootEffect.ResetFeedbacks();
        ShootEffect.RestoreInitialValues();
        ShootEffect.PlayFeedbacks();
    }

    private void OnDrawGizmos()
    {
        if (_config == null) return;

        Gizmos.color = Color.purple;
        Vector3 rayStartPosition = Transform.localPosition.ResetX(_config.LocalMoveBounds * -1);
        Gizmos.DrawRay(rayStartPosition, Vector3.right * _config.LocalMoveBounds * 2);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Transform.localPosition + _config.ShootPositionOffset, 0.1f);
    }
}
