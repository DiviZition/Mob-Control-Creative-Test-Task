using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Damageable _damageable;
    [SerializeField] private Transform _hpBarTransform;
    [SerializeField] private RectMask2D _fillMask;
    [SerializeField] private TMP_Text _healthCounter;

    private Camera _mainCamera;

    private void OnEnable()
    {
        _mainCamera = Camera.main;
        _damageable.OnTakeDamage += UpdateHPBar;
        UpdateHPBar();
    }

    private void OnDisable()
    {
        _damageable.OnTakeDamage -= UpdateHPBar;
    }

    void LateUpdate()
    {
        // Billboard effect: face the camera
        if (_mainCamera != null)
        {
            _hpBarTransform.forward = _mainCamera.transform.forward;
        }
    }

    public void UpdateHPBar() => SetupHPBar(_damageable.MaxHealth, _damageable.CurrentHealth);
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
