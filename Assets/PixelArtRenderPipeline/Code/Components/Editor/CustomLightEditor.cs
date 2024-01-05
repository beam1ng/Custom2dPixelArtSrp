using UnityEditor;

namespace PixelArtRenderPipeline.Code.Components.Editor
{
    [CustomEditor(typeof(CustomLight))]
    public class CustomLightEditor : UnityEditor.Editor
    {
        private SerializedProperty _lightType;
        private SerializedProperty _intensity;
        private SerializedProperty _color;
        private SerializedProperty _range;
    
        private void OnEnable()
        {
            _lightType = serializedObject.FindProperty("<Type>k__BackingField");
            _intensity = serializedObject.FindProperty("<Intensity>k__BackingField");
            _color = serializedObject.FindProperty("<Color>k__BackingField");
            _range = serializedObject.FindProperty("<Range>k__BackingField");
        }
    
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            EditorGUILayout.PropertyField(_lightType);
            EditorGUILayout.PropertyField(_color);
            EditorGUILayout.PropertyField(_intensity);
            if ((CustomLight.LightType)_lightType.enumValueIndex == CustomLight.LightType.Point)
            {
                EditorGUILayout.PropertyField(_range);
            }
        
            // Apply the modified properties to the serializedObject
            serializedObject.ApplyModifiedProperties();
        }
    }
}