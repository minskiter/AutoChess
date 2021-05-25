using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoSave
{
    // Static constructor that gets called when unity fires up.
    static AutoSave()
    {
        EditorApplication.playModeStateChanged += (PlayModeStateChange state) => {
            // If we're about to run the scene...
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                // Save the scene and the assets.
                Debug.Log("Auto-saving all open scenes... " + state);
                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
            }
        };
    }
}