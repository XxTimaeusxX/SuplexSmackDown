using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HealthPickUp))]
public class HealthConfigEditor : Editor
{
    string[] healthPickupTypes = new string[] { "Default", "Drink" };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        SerializedProperty healthIndex = serializedObject.FindProperty("healthIndex");

        healthIndex.intValue = EditorGUILayout.Popup(
            "Health Type",
            healthIndex.intValue,
            healthPickupTypes
        );

        serializedObject.ApplyModifiedProperties();
    }

}
