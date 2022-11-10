using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameSettingsManager))]
public class GameSettingsEditor : Editor
{
    private GameSettingsManager manager;

    private void OnEnable()
    {
        manager = (GameSettingsManager)target;
    }

    public override void OnInspectorGUI()
    {
        if (manager.lightVersion == false) {
            base.OnInspectorGUI();
        } else {
            serializedObject.Update();
            manager.lightVersion = EditorGUILayout.Toggle("Light Version", manager.lightVersion);

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Настройки шейдера", EditorStyles.boldLabel);

            manager.glitchMaterial = (Material)EditorGUILayout.ObjectField("Glitch material", manager.glitchMaterial, typeof(Material), true);
            manager.noGlitchMaterial = (Material)EditorGUILayout.ObjectField("No glitch material", manager.noGlitchMaterial, typeof(Material), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("quads"), true);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("cursorsTextures"), true);

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Настройки счётчика fps", EditorStyles.boldLabel);

            manager.fpsCounter = (FpsCounter)EditorGUILayout.ObjectField("Fps counter", manager.fpsCounter, typeof(FpsCounter), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
