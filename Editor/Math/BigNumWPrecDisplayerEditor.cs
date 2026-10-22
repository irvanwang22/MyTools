using UnityEditor;
using TMPro.EditorUtilities;

namespace MyTools.Math.Editor
{
    [CustomEditor(typeof(BigNumWPrecDisplayer), true)]
    public class BigNumWPrecDisplayerEditor : TMP_EditorPanelUI
    {
        private SerializedProperty standardLimitProp;
        private SerializedProperty significantDigitsProp;
        private SerializedProperty withCommaProp;

        protected override void OnEnable()
        {
            base.OnEnable();
            standardLimitProp = serializedObject.FindProperty("standardLimit");
            significantDigitsProp = serializedObject.FindProperty("significantDigits");
            withCommaProp = serializedObject.FindProperty("withComma");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Big Number Display", EditorStyles.boldLabel);
            serializedObject.Update();
            EditorGUILayout.PropertyField(standardLimitProp);
            EditorGUILayout.PropertyField(significantDigitsProp);
            EditorGUILayout.PropertyField(withCommaProp);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
