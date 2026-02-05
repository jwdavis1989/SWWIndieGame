using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlayFromStartScene
{
    static PlayFromStartScene()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Set the scene to the one at build index 0
            if (EditorBuildSettings.scenes.Length > 0)
            {
                string startScenePath = EditorBuildSettings.scenes[0].path;
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(startScenePath);
            }
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            // Optional: Clear the start scene when returning to edit mode
            EditorSceneManager.playModeStartScene = null;
        }
    }
}
