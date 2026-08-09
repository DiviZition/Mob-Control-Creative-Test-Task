using MoreMountains.Feedbacks;
using UnityEngine;

public class FeelVFXOnDamageTaken : MonoBehaviour
{
    [SerializeField] private MMF_Player _feelPlayer;
    [SerializeField] private DamageableView _damageable;
    [SerializeField] private bool _resetFeelPlayer;

    public void PlayVFX(int _)
    {
        if (_resetFeelPlayer)
        {
            _feelPlayer.StopFeedbacks();
            _feelPlayer.RestoreInitialValues();
        }

        _feelPlayer.PlayFeedbacks();
    }

    private void OnEnable() => _damageable.OnDamageTaken += PlayVFX;
    private void OnDisable() => _damageable.OnDamageTaken -= PlayVFX;
}
