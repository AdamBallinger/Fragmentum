using Assets.Scripts;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Editor
{
    [CustomEditor(typeof(ProgressiveSceneLoader))]
    [CanEditMultipleObjects]
    public class ProgressiveSceneLoaderEditor : UnityEditor.Editor
    {

        private ProgressiveSceneLoader Target
        {
            get { return (ProgressiveSceneLoader) target; }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Loader Settings", EditorStyles.helpBox);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("delayBetweenStatic"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("delayBetweenDynamic"), true);

            EditorGUILayout.LabelField("Static Object Options", EditorStyles.helpBox);

            if(GUILayout.Button("Enable Static Objects"))
            {
                foreach(var staticObj in Target.staticObjects)
                {
                    staticObj.SetActive(true);
                }

                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Disable Static Objects"))
            {
                foreach (var staticObj in Target.staticObjects)
                {
                    staticObj.SetActive(false);
                }

                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            EditorGUILayout.LabelField("Dynamic Object Options", EditorStyles.helpBox);

            if (GUILayout.Button("Enable Dynamic Objects"))
            {
                foreach (var dynObj in Target.dynamicObjects)
                {
                    dynObj.SetActive(true);
                }

                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Disable Dynamic Objects"))
            {
                foreach (var dynObj in Target.dynamicObjects)
                {
                    dynObj.SetActive(false);
                }

                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            EditorGUILayout.LabelField("Object List Options", EditorStyles.helpBox);

            if (GUILayout.Button("Clear Static Objects"))
            {
                Target.staticObjects.Clear();
            }

            if (GUILayout.Button("Clear Dynamic Objects"))
            {
                Target.dynamicObjects.Clear();
            }

            EditorGUILayout.LabelField("Object Lists", EditorStyles.helpBox);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("staticObjects"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dynamicObjects"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
