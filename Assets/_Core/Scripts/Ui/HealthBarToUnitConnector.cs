using UnityEngine;

public class HealthBarToUnitConnector : MonoBehaviour
{
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private UnitView _unitView;

    private void OnValidate()
    {
        _healthBar = _healthBar ?? GetComponent<HealthBar>();
        _unitView = _unitView ?? GetComponent<UnitView>();
    }

    private void Start()
    {

    }
}
