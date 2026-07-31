#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class UnitRedirectionTrigger : MonoBehaviour
{
    [SerializeField] private UnitBattleSide _whoToRedirect;

    [SerializeField] private bool _overrideDirection;
    [SerializeField] private Transform _directionReference;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UnitBase unit) && unit.BattleSide == _whoToRedirect)
        {
            var rotation = _overrideDirection ? _directionReference.rotation : unit.transform.rotation;
            unit.Movement.RotateUnit(rotation);
        }
    }
}

[CustomEditor(typeof(UnitRedirectionTrigger))]
public class UnitRedirectionTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_whoToRedirect"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_overrideDirection"));

        if (serializedObject.FindProperty("_overrideDirection").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_directionReference"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}