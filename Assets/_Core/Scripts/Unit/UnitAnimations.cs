using PrimeTween;
using System;
using UnityEngine;

public class UnitAnimations : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private UnitAnimationData[] _animations;

    [SerializeField] private Material _normalMaterial;
    [SerializeField] private Material _deadMaterial;
    [SerializeField] private SkinnedMeshRenderer _unitMeshRenderer;

    private IUnitAttack _unitAttack;
    private bool _playAttackAnimation;

    public void Init(bool playsAttackAnimation, IUnitAttack unitAttack)
    {
        _unitAttack = unitAttack;
        _playAttackAnimation = playsAttackAnimation;
        PlayRunAnimation();

        if (_playAttackAnimation)
        {
            _unitAttack.OnAttackStarted += PlayAttackAnimation;
            _unitAttack.OnAttackFinished += PlayRunAnimation;
        }
    }

    private void OnDestroy()
    {
        if (_playAttackAnimation)
        {
            _unitAttack.OnAttackStarted -= PlayAttackAnimation;
            _unitAttack.OnAttackFinished -= PlayRunAnimation;
        }
    }

    public void PlayRunAnimation()
    {
        if (TryGetAnimDataIndex(AnimationType.Run, out int animDataIndex) == false)
            PlayAnimation(_animations[animDataIndex].StateName);
    }

    public void PlayAttackAnimation()
    {
        if (TryGetAnimDataIndex(AnimationType.Attack, out int animDataIndex) == false)
            PlayAnimation(_animations[animDataIndex].StateName);
    }

    public void PlayDeadAnimation(Action onFinished = null)
    {
        if (TryGetAnimDataIndex(AnimationType.Die, out int animDataIndex) == false)
            return;

        PlayAnimation(_animations[animDataIndex].StateName);
        SetDeadMaterial();
        var unitTransform = transform;
        float initialYPos = unitTransform.localPosition.y;
        float animDuration = _animations[animDataIndex].DeclaredDuration;
        float dissapearDuration = _animations[animDataIndex].DeclaredDuration;
        Sequence.Create()
            .Group(Tween.LocalPositionY(unitTransform, endValue: initialYPos - 0.5f, duration: dissapearDuration, startDelay: animDuration))
            .ChainCallback(() => onFinished?.Invoke())
            .ChainCallback(() => SetNormalMaterial());
    }
    private void PlayAnimation(string stateName, float transitionDuration = 0.2f) => _animator.CrossFade(stateName, transitionDuration);

    private bool TryGetAnimDataIndex(AnimationType animationType, out int index)
    {
        index = -1;
        for (int i = 0; i < _animations.Length; i++)
        {
            if (_animations[i].Type == animationType)
            {
                index = i;
                return true;
            }
        }

        Debug.LogError($"No animation of type: {animationType} on unit: {this.gameObject.name}");
        return false;
    }

    public void SetDeadMaterial() => _unitMeshRenderer.material = _deadMaterial;
    public void SetNormalMaterial() => _unitMeshRenderer.material = _normalMaterial;

    [Serializable]
    private struct UnitAnimationData
    {
        public AnimationType Type;
        public string StateName;
        public float DeclaredDuration;
    }
}

public enum AnimationType : sbyte
{
    None = -1,
    Run = 0,
    Die = 1,
    Attack = 2,
}
