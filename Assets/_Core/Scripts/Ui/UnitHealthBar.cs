using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform _hpBarTransform;
    [SerializeField] private RectMask2D _fillMask;
    [SerializeField] private TMP_Text _healthCounter;

    private Health _health;

    private Camera _mainCamera;

    private void OnEnable()
    {
        _mainCamera = Camera.main;
        _health.OnHealthChanged += UpdateHPBar;
        UpdateHPBar();
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= UpdateHPBar;
    }

    void LateUpdate()
    {
        // Billboard effect: face the camera
        if (_mainCamera != null)
        {
            _hpBarTransform.forward = _mainCamera.transform.forward;
        }
    }

    public void UpdateHPBar(int _ = 0) => SetupHPBar(_health.MaxHealth, _health.CurrentHealth);
    private void SetupHPBar(int maxHp, int currentHP)
    {
        bool isFullHP = currentHP == maxHp;
        _hpBarTransform.gameObject.SetActive(isFullHP == false);

        if (isFullHP == true || _fillMask == null)
            return;

        _healthCounter.text = currentHP.ToString();

        float ratio = (float)currentHP / (float)maxHp;
        // Calculate how much to hide (in pixels)
        float hiddenWidth = _fillMask.rectTransform.rect.width * (1f - ratio);
        // Apply it to the left padding
        _fillMask.padding = new Vector4(0f, 0f, hiddenWidth, 0f);
    }
}
