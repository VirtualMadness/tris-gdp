using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Movement2D))]
[CanEditMultipleObjects]
public class Movement2DCustomEditor : Editor
{
    private SerializedProperty radiusProp;
    private SerializedProperty pivotProp;
    private SerializedProperty sizeProp;

    private void OnEnable()
    {
        radiusProp = serializedObject.FindProperty("radius");
        pivotProp = serializedObject.FindProperty("offsetFromPivot");
        sizeProp = serializedObject.FindProperty("size");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        var m = ((Movement2D)target);
        GUI.enabled = !m.GetComponent<Collider>();
        if (GUILayout.Button("Calculate Properties"))
        {
            if (m.shapeMode == Movement2D.ShapeMode.Circle)
            {
                var s = m.gameObject.AddComponent<CircleCollider2D>();
                radiusProp.floatValue = s.radius * Mathf.Max(m.transform.localScale.x, m.transform.localScale.z);
                pivotProp.vector2Value = s.offset;
                GameObject.DestroyImmediate(s);
            }
            else if (m.shapeMode == Movement2D.ShapeMode.Box)
            {
                var c = m.gameObject.AddComponent<BoxCollider2D>();
                sizeProp.vector2Value = c.size * Mathf.Max(m.transform.localScale.x, m.transform.localScale.z);
                pivotProp.vector2Value = c.offset;
                GameObject.DestroyImmediate(c);
            }
        }
        GUI.enabled = true;
        serializedObject.ApplyModifiedProperties();
    }
}