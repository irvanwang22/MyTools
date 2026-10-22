using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public class CreateScriptableInstanceFromScript
{
    [MenuItem("Assets/Create/ScriptableObject From Script", true)]
    static bool ValidateCreateSO()
    {
        return Selection.activeObject is MonoScript script && typeof(ScriptableObject).IsAssignableFrom(script.GetClass());
    }

    [MenuItem("Assets/Create/ScriptableObject From Script")]
    static void CreateScriptableObject()
    {
        MonoScript script = Selection.activeObject as MonoScript;
        Type scriptClass = script.GetClass();

        if (scriptClass == null || !typeof(ScriptableObject).IsAssignableFrom(scriptClass))
        {
            Debug.LogError("Selected script is not a valid ScriptableObject class.");
            return;
        }

        ScriptableObject asset = ScriptableObject.CreateInstance(scriptClass);

        string path = AssetDatabase.GetAssetPath(script);
        string folder = Path.GetDirectoryName(path);
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folder, scriptClass.Name + ".asset"));

        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
