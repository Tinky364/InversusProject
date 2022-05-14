using UnityEditor;
using UnityEditor.UI;

using Inversus.UI;

namespace Inversus.Editor
{
    [CustomEditor(typeof(ButtonX), true)]
    public class ButtonXEditor : SelectableEditor
    {
        private SerializedProperty _buttonExecuted;
        private SerializedProperty _hasText;
        private SerializedProperty _textMesh;

        protected override void OnEnable()
        {
            base.OnEnable();
            _buttonExecuted = serializedObject.FindProperty("ButtonExecuted");
            _hasText = serializedObject.FindProperty("_hasText");
            _textMesh = serializedObject.FindProperty("_textMesh");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_buttonExecuted);
            EditorGUILayout.PropertyField(_hasText);
            ++EditorGUI.indentLevel;
            {
                if (_hasText.boolValue)
                    EditorGUILayout.PropertyField(_textMesh);
            }
            --EditorGUI.indentLevel;

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
