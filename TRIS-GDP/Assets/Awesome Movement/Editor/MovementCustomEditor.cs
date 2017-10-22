using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Movement))]
[CanEditMultipleObjects]
public class MovementCustomEditor : Editor
{
    private SerializedProperty radiusProp;
    private SerializedProperty pivotProp;
    private SerializedProperty heightProp;

    private void OnEnable()
    {
        radiusProp = serializedObject.FindProperty("radius");
        pivotProp = serializedObject.FindProperty("offsetFromPivot");
        heightProp = serializedObject.FindProperty("height");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        var m = ((Movement)target);
        GUI.enabled = !m.GetComponent<Collider>();
        if (GUILayout.Button("Calculate Properties"))
        {
            if (m.shapeMode == Movement.ShapeMode.Sphere)
            {
                var s = m.gameObject.AddComponent<SphereCollider>();
                radiusProp.floatValue = s.radius * Mathf.Max(m.transform.localScale.x, m.transform.localScale.z);
                pivotProp.vector3Value = s.center;
                GameObject.DestroyImmediate(s);
            }
            else if (m.shapeMode == Movement.ShapeMode.Capsule)
            {
                var c = m.gameObject.AddComponent<CapsuleCollider>();
                radiusProp.floatValue = c.radius * Mathf.Max(m.transform.localScale.x, m.transform.localScale.z);
                pivotProp.vector3Value = c.center;
                heightProp.floatValue = c.height * Mathf.Max(m.transform.localScale.x, m.transform.localScale.z);
                GameObject.DestroyImmediate(c);
            }
        }
        GUI.enabled = true;
        serializedObject.ApplyModifiedProperties();
    }
}