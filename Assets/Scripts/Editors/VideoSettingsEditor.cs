using UnityEditor;

[CustomEditor(typeof(VideoSettingsManager))]
public class VideoSettingsEditor : Editor
{
    private VideoSettingsManager manager;

    private void OnEnable()
    {
        manager = (VideoSettingsManager)target;
    }

    public override void OnInspectorGUI()
    {
        if (manager.lightVersion == false) {
            base.OnInspectorGUI();
        } else {
            serializedObject.Update();
            manager.lightVersion = EditorGUILayout.Toggle("Light Version", manager.lightVersion);


            EditorGUILayout.PropertyField(serializedObject.FindProperty("resolutionsScreen"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("screenMods"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("textures"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
