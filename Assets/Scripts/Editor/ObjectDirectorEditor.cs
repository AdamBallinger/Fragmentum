using Assets.Scripts.CameraUtils;
using Assets.Scripts.Objects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Editor
{
    [CustomEditor(typeof(ObjectDirector))]
    [CanEditMultipleObjects]
    public class ObjectDirectorEditor : UnityEditor.Editor
    {

        public ObjectDirector Target
        {
            get { return (ObjectDirector)serializedObject.targetObject; }
        }

        public ObjectDirector TargetSingle
        {
            get { return (ObjectDirector)target; }
        }

        private ReorderableList reorderList;
        private bool listFoldout = false;

        private void OnEnable()
        {
            reorderList = new ReorderableList(serializedObject, serializedObject.FindProperty("directorPoints"), true, true, true, true)
            {
                elementHeightCallback = (_index =>
                {
                    Repaint();

                    var size = EditorGUIUtility.singleLineHeight * 4.0f;

                    if (Target.directorPoints[_index].enableCallback)
                    {
                        size += EditorGUIUtility.singleLineHeight * 6.0f;

                        if (Target.directorPoints[_index].onReached != null)
                        {
                            if (Target.directorPoints[_index].onReached.GetPersistentEventCount() > 1)
                                size += (Target.directorPoints[_index].onReached.GetPersistentEventCount() - 1) * (EditorGUIUtility.singleLineHeight * 2.75f);
                        }
                    }
                    else
                    {
                        size += EditorGUIUtility.singleLineHeight * 1.25f;
                    }

                    if (Target.directorPoints[_index].enableCustomCurve)
                    {
                        size += EditorGUIUtility.singleLineHeight * 1.75f;
                    }

                    return size;
                })
            };

            // Change how each element in the list is drawn.
            reorderList.drawElementCallback = (_rect, _index, _isActive, _isFocused) =>
            {
                var element = reorderList.serializedProperty.GetArrayElementAtIndex(_index);

                // Draw the name of the element.
                EditorGUI.LabelField(new Rect(_rect.x, _rect.y + 1.0f, _rect.width, EditorGUIUtility.singleLineHeight),
                    "Cutscene Point: " + _index, EditorStyles.boldLabel);

                _rect.y += EditorGUIUtility.singleLineHeight + 7.0f;

                // Draws a box at end of element for the delay time.
                EditorGUI.PropertyField(new Rect(_rect.x + _rect.width - 150.0f, _rect.y - 20.0f, 25.0f, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("delay"), GUIContent.none);

                EditorGUI.LabelField(new Rect(_rect.x + _rect.width - 190.0f, _rect.y - 20.0f, 60.0f, EditorGUIUtility.singleLineHeight), "Delay");

                // Draws a box at end of element for the transition time.
                EditorGUI.PropertyField(new Rect(_rect.x + _rect.width - 30.0f, _rect.y - 20.0f, 25.0f, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("transitionTime"), GUIContent.none);

                EditorGUI.LabelField(new Rect(_rect.x + _rect.width - 100.0f, _rect.y - 20.0f, 70.0f, EditorGUIUtility.singleLineHeight), "Move Time");

                // Draw a button to set object pos and rotation to elements values
                if (GUI.Button(new Rect(_rect.x + _rect.width - 135.0f, _rect.y + 10.0f, 60.0f, EditorGUIUtility.singleLineHeight), "Move To"))
                {
                    Target.directedObject.transform.position = Target.directorPoints[_index].objectPosition;
                    Target.directedObject.transform.rotation = Quaternion.Euler(Target.directorPoints[_index].objectRotation);
                }

                if (GUI.Button(new Rect(_rect.x + _rect.width - 65.0f, _rect.y + 10.0f, 60.0f, EditorGUIUtility.singleLineHeight), "Update"))
                {
                    Target.directorPoints[_index].objectPosition = Target.directedObject.transform.position;
                    Target.directorPoints[_index].objectRotation = Target.directedObject.transform.rotation.eulerAngles;
                }


                EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("haltOnReach"), GUIContent.none);
                EditorGUI.LabelField(new Rect(_rect.x + 15.0f, _rect.y, 140.0f, EditorGUIUtility.singleLineHeight), "Halt When Reached");

                _rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("enableCustomCurve"), GUIContent.none);
                EditorGUI.LabelField(new Rect(_rect.x + 15.0f, _rect.y, 160.0f, EditorGUIUtility.singleLineHeight), "Enable Custom Curve");

                _rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                if (Target.directorPoints[_index].enableCustomCurve)
                {
                    EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, _rect.width,
                        EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("moveCurve"), new GUIContent("Curve"));

                    _rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                }

                EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("enableCallback"), GUIContent.none);
                EditorGUI.LabelField(new Rect(_rect.x + 15.0f, _rect.y, 160.0f, EditorGUIUtility.singleLineHeight), "Enable On Reach Callback");

                _rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                if (Target.directorPoints[_index].enableCallback)
                {
                    EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, _rect.width,
                        EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("onReached"), new GUIContent("On Reached"));
                }
            };

            // Sets name in the header for the list.
            reorderList.drawHeaderCallback = (_rect) => { EditorGUI.LabelField(_rect, "Director Points"); };

            EditorUtility.SetDirty(Target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Debug Settings", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableGizmos"), true);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Director Settings", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("directedObject"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startOnInitialize"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultCurve"), true);

            if (GUILayout.Button("Move Object to Start"))
            {
                if (Target.directorPoints != null && Target.directorPoints.Count > 0)
                {
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    Target.directedObject.transform.position = Target.directorPoints[0].objectPosition;
                    Target.directedObject.transform.rotation = Quaternion.Euler(Target.directorPoints[0].objectRotation);
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Director Controls", EditorStyles.helpBox);

            // Button to add the current object position and rotation to the list.
            if (GUILayout.Button("Add New"))
            {
                var directorData = new DirectorData
                {
                    objectPosition = Target.directedObject.transform.position,
                    objectRotation = Target.directedObject.transform.rotation.eulerAngles
                };
                Target.directorPoints.Add(directorData);
            }

            // Button to clear the list.
            if (GUILayout.Button("Clear Director"))
            {
                if (EditorUtility.DisplayDialog("Confirm Clear",
                    "Are you sure you want to clear the director points? This process can not be undone.", "Yes", "No"))
                {
                    reorderList.serializedProperty.ClearArray();
                }
            }

            listFoldout = EditorGUILayout.Foldout(listFoldout, "Director Points");

            if (listFoldout)
            {
                reorderList.DoLayoutList();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Director State Callbacks", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onStart"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onPointReached"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onFinish"), true);

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            if (TargetSingle.directorPoints == null) return;

            if (!TargetSingle) return;

            var guiStyle = new GUIStyle();
            guiStyle.normal.textColor = Color.white;
            guiStyle.fontSize = 20;

            for (var i = 0; i < TargetSingle.directorPoints.Count; i++)
            {
                Handles.Label(TargetSingle.directorPoints[i].objectPosition, i.ToString(), guiStyle);
            }
        }
    }
}