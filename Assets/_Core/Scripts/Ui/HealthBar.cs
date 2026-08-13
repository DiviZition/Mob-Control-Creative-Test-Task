using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform _hpBarTransform;
    [SerializeField] private RectMask2D _fillMask;
    [SerializeField] private TMP_Text _healthCounter;

    private Transform _cameraTransform;
    private IHealth _health;

    public void Init(IHealth health)
    {
        _health = health;

        _cameraTransform = Camera.main.transform;
        _health.OnHealthChanged += SetupHPBar;
    }

    private void OnEnable() => SetupHPBar(1, 1);
    private void OnDestroy() => _health.OnHealthChanged -= SetupHPBar;

    void LateUpdate()
    {
        // Billboard effect: face the camera
        if (_cameraTransform != null)
        {
            _hpBarTransform.forward = _cameraTransform.forward;
        }
    }

    private void SetupHPBar(int maxHP, int currentHP)
    {
        bool isFullHP = currentHP == maxHP;
        _hpBarTransform.gameObject.SetActive(isFullHP == false);

        if (isFullHP == true || _fillMask == null)
            return;

        _healthCounter.text = currentHP.ToString();

        float ratio = (float)currentHP / (float)maxHP;
        // Calculate how much to hide (in pixels)
        float hiddenWidth = _fillMask.rectTransform.rect.width * (1f - ratio);
        // Apply it to the left padding
        _fillMask.padding = new Vector4(0f, 0f, hiddenWidth, 0f);
    }
}
