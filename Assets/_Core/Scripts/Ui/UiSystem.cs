using PrimeTween;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UiSystem : MonoBehaviour
{
    [SerializeField] private PopUpTextScreenData _defeatedScreenData;
    [SerializeField] private PopUpTextScreenData _youWonScreenData;

    private void Awake() => HideAllScreens();

    public void HideAllScreens()
    {
        _defeatedScreenData.Canvas.gameObject.SetActive(false);
        _youWonScreenData.Canvas.gameObject.SetActive(false);
    }

    [ContextMenu("Show Defeated Screen")]
    public void ShowDefeatedScreen() => PopUpTextScreen(ref _defeatedScreenData);

    [ContextMenu("Show You Won Screen")]
    public void ShowYouWonScreen() => PopUpTextScreen(ref _youWonScreenData);

    public void PopUpTextScreen(ref PopUpTextScreenData data)
    {
        HideAllScreens();
        data.Image.transform.localScale = data.StartAnimationSize;
        data.Canvas.gameObject.SetActive(true);

        Tween.Scale(
            target: data.Image.transform,
            startValue: data.StartAnimationSize,
            endValue: data.EndAnimationSize,
            duration: data.AnimationDuration,
            ease: data._easing);
    }
}

[Serializable]
public struct PopUpTextScreenData
{
    public Canvas Canvas;
    public Image Image;
    public Vector3 StartAnimationSize;
    public Vector3 EndAnimationSize;
    public float AnimationDuration;
    public Ease _easing;
}