using PrimeTween;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UiSystem : MonoBehaviour
{
    [SerializeField] private DefeatedScreenData _defeatedScreenData;

    private void Awake() => HideAllScreens();

    public void HideAllScreens()
    {
        _defeatedScreenData.Canvas.gameObject.SetActive(false);
    }

    [ContextMenu("Show Defeated Screen")]
    public void ShowDefeatedScreen()
    {
        _defeatedScreenData.Image.transform.localScale = _defeatedScreenData.StartAnimationSize;
        _defeatedScreenData.Canvas.gameObject.SetActive(true);

        Tween.Scale(
            target: _defeatedScreenData.Image.transform,
            startValue: _defeatedScreenData.StartAnimationSize,
            endValue: _defeatedScreenData.EndAnimationSize,
            duration: _defeatedScreenData.AnimationDuration,
            ease: _defeatedScreenData._easing);
    }
}

[Serializable]
public struct DefeatedScreenData
{
    public Canvas Canvas;
    public Image Image;
    public Vector3 StartAnimationSize;
    public Vector3 EndAnimationSize;
    public float AnimationDuration;
    public Ease _easing;
}