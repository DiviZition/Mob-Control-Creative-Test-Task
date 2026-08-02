using PrimeTween;
using R3;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitView : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private UnitAnimationData[] _animations;

    [SerializeField] private Material _normalMaterial;
    [SerializeField] private Material _deadMaterial;
    [SerializeField] private SkinnedMeshRenderer _unitMeshRenderer;

    public void SetDeadMaterial() => _unitMeshRenderer.material = _deadMaterial;
    public void SetNormalMaterial() => _unitMeshRenderer.material = _normalMaterial;

    public void PlayAnimation(AnimationType animType, Action onAnimEnded = null)
    {
        if (TryGetAnimDataIndex(animType, out int animDataIndex) == false)
            return;

        PlayAnimation(_animations[animDataIndex].StateName, _animations[animDataIndex].DeclaredDuration);
        if (onAnimEnded != null)
            Tween.Delay(_animations[animDataIndex].DeclaredDuration, () => onAnimEnded?.Invoke());
    }

    public void PlayDeadAnimation(Action onAnimEnded = null)
    {
        if (TryGetAnimDataIndex(AnimationType.Die, out int animDataIndex) == false)
            return;

        PlayAnimation(_animations[animDataIndex].StateName);
        SetDeadMaterial();
        var unitTransform = transform;
        float initialYPos = unitTransform.localPosition.y;
        float animDuration = _animations[animDataIndex].DeclaredDuration;
        Sequence.Create()
            .Group(Tween.LocalPositionY(unitTransform, endValue: initialYPos - 0.5f, duration: 5, startDelay: animDuration))
            .ChainCallback(() => onAnimEnded?.Invoke())
            .ChainCallback(() => SetNormalMaterial());
    }
    protected void PlayAnimation(string stateName, float transitionDuration = 0.2f) => _animator.CrossFade(stateName, transitionDuration);

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

    [Serializable]
    private struct UnitAnimationData
    {
        public AnimationType Type;
        public string StateName;
        public float DeclaredDuration;
    }
}

public enum AnimationType
{
    Run,
    Die,
    Attack,
}
