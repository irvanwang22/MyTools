using System.Globalization;
using System.Numerics;
using UnityEditor;
using UnityEngine;

namespace MyTools.Math.Editor
{
    [CustomPropertyDrawer(typeof(BigNumWPrec))]
    public class BigNumWPrecPropertyDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + Spacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var valueStrProp = property.FindPropertyRelative("_valueStr");
            var precisionProp = property.FindPropertyRelative("precision");

            float lineH = EditorGUIUtility.singleLineHeight;

            // Row 1: value
            Rect valueRect = new Rect(position.x, position.y, position.width, lineH);
            string displayStr = BuildDisplay(valueStrProp.stringValue, (uint)precisionProp.intValue);
            EditorGUI.BeginChangeCheck();
            string newStr = EditorGUI.TextField(valueRect, label, displayStr);
            if (EditorGUI.EndChangeCheck())
            {
                string parsed = TryParseToValueStr(newStr);
                if (parsed != null)
                    valueStrProp.stringValue = parsed;
            }

            // Row 2: precision (indented)
            Rect precisionRect = new Rect(position.x, position.y + lineH + Spacing, position.width, lineH);
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(precisionRect, precisionProp);
            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        private string BuildDisplay(string valueStr, uint precision)
        {
            if (string.IsNullOrEmpty(valueStr) || valueStr == "0e0") return "0";
            int eIdx = valueStr.IndexOf('e');
            if (eIdx < 0) return valueStr;
            if (!BigInteger.TryParse(valueStr[..eIdx], out var mantissa)) return valueStr;
            if (!int.TryParse(valueStr[(eIdx + 1)..], out int exponent)) return valueStr;
            return new BigNumWPrec(mantissa, exponent, precision == 0 ? 90 : precision)
                .ToAutoDisplayNotation(12, 4, false);
        }

        // Accepts "1000000", "1e6", "1.5e9", etc. Returns internal "mantissaeexponent" string.
        private string TryParseToValueStr(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "0e0";
            if (!double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
                return null;
            return new BigNumWPrec(val).ToString();
        }
    }
}
