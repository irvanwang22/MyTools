using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace MyTools.Editor
{
    public class TMPFontReplacer : EditorWindow
    {
        TMP_FontAsset newFont;

        [MenuItem("Tools/Replace TMP Font in Current Scene")]
        static void ShowWindow()
        {
            GetWindow<TMPFontReplacer>("TMP Font Replacer");
        }

        void OnGUI()
        {
            newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New TMP Font", newFont, typeof(TMP_FontAsset), false);

            if (GUILayout.Button("Replace Fonts In Current Scene"))
            {
                if (newFont == null)
                {
                    Debug.LogError("Please select a TMP_FontAsset first!");
                    return;
                }

                ReplaceFontsInCurrentScene(newFont);
            }
        }

        static void ReplaceFontsInCurrentScene(TMP_FontAsset font)
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            if (!scene.isLoaded)
            {
                Debug.LogError("No active scene loaded!");
                return;
            }

            TMP_Text[] texts = FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);

            foreach (TMP_Text tmp in texts)
            {
                if (tmp.gameObject.scene == scene)
                {
                    Undo.RecordObject(tmp, "Replace TMP Font");
                    tmp.font = font;
                    EditorUtility.SetDirty(tmp);
                }
            }

            EditorSceneManager.SaveScene(scene);
            Debug.Log($"Replace completed: {scene.path}");
        }
    }
}