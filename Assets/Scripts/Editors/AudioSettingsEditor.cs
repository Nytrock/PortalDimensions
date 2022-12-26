using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(AudioSettingsManager))]
public class AudioSettingsEditor : Editor
{
    private AudioSettingsManager manager;

    private void OnEnable()
    {
        manager = (AudioSettingsManager)target;
    }

    public override void OnInspectorGUI()
    {
        if (manager.lightVersion == false) {
            base.OnInspectorGUI();
        } else {
            serializedObject.Update();
            manager.lightVersion = EditorGUILayout.Toggle("Light Version", manager.lightVersion);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("musicMixer"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectsMixer"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("uiMixer"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif