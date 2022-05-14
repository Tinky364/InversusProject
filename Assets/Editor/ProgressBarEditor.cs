using UnityEditor;

using Inversus.UI;

namespace Inversus.Editor
{
    [CustomEditor(typeof(ProgressBar), true)]
    public class ProgressBarEditor : UnityEditor.Editor
    {
        private SerializedProperty _fillMethod;
        private SerializedProperty _horizontalFillOrigin;
        private SerializedProperty _verticalFillOrigin;

        private void OnEnable()
        {
            _fillMethod = serializedObject.FindProperty("_fillMethod");
            _horizontalFillOrigin = serializedObject.FindProperty("_horizontalFillOrigin");
            _verticalFillOrigin = serializedObject.FindProperty("_verticalFillOrigin");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            switch (_fillMethod.enumValueIndex)
            {
                case (int) ProgressBar.FillMethod.Horizontal:
                    EditorGUILayout.PropertyField(_horizontalFillOrigin);
                    break;
                case (int) ProgressBar.FillMethod.Vertical:
                    EditorGUILayout.PropertyField(_verticalFillOrigin);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
