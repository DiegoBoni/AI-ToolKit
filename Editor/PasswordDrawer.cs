#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// This class defines how a property with the [Password] attribute is drawn in the Inspector.
/// </summary>
[CustomPropertyDrawer(typeof(PasswordAttribute))]
public class PasswordDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Ensure this is only used on string fields
        if (property.propertyType == SerializedPropertyType.String)
        {
            // Use a PasswordField to draw the property, which masks the input
            property.stringValue = EditorGUI.PasswordField(position, label, property.stringValue);
        }
        else
        {
            // If not a string, show a warning in the Inspector
            EditorGUI.LabelField(position, label.text, "Use [Password] attribute only on string fields.");
        }
    }
}
#endif