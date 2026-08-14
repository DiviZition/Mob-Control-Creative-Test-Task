using MoreMountains.Feedbacks;
using UnityEngine;

public class CanonView : MonoBehaviour
{
    [field: SerializeField] public Transform Transform {  get; private set; }
    [field: SerializeField] public CanonSettingsConfig CanonConfig {  get; private set; }
    [field: SerializeField] public UnitConfig PlayerUnitConfig {  get; private set; }

    [field: SerializeField] public MMF_Player ShootEffect {  get; private set; }

    [SerializeField] private CanonSettingsConfig _config;
    private Vector3 _canonPositionForGizmos;

    public ICanonModel _canonModel;

    public void Init(ICanonModel canonModel)
    {
        _canonModel = canonModel;
        _canonModel.Shooter.OnCanonShoot += PlayShootEffect;
    }

    private void OnDestroy()
    {
        if (_canonModel != null)
            _canonModel.Shooter.OnCanonShoot += PlayShootEffect;
    }

    private void Update()
    {
        if (_canonModel == null || _canonModel.IsDisabled) return;

        Transform.position = _canonModel.Movement.CurrentPosition;
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

        if (Application.isPlaying == false)
            _canonPositionForGizmos = Transform.position;

        Gizmos.color = Color.purple;
        Vector3 rayStartPosition = _canonPositionForGizmos + (Vector3.right * _config.LocalMoveBounds * -1);
        Gizmos.DrawRay(rayStartPosition, Vector3.right * _config.LocalMoveBounds * 2);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Transform.position + _config.ShootPositionOffset, _config.ShootSpread);
    }
}
