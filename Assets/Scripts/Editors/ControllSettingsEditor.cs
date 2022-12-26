using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(ControllSettingsManager))]
public class ControllSettingsEditor : Editor
{
    private ControllSettingsManager manager;

    private void OnEnable()
    {
        manager = (ControllSettingsManager)target;
    }

    public override void OnInspectorGUI()
    {
        if (manager.lightVersion == false) {
            base.OnInspectorGUI();
        } else {
            serializedObject.Update();
            manager.lightVersion = EditorGUILayout.Toggle("Light Version", manager.lightVersion);


            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftDefault"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightDefault"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpDefault"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftPortalDefault"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightPortalDefault"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogueDefault"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("restartDefault"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif