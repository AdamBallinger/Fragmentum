using Assets.Scripts.CameraUtils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Editor
{
    [CustomEditor(typeof(CameraCutscene))]
    [CanEditMultipleObjects]
    public class CameraCutsceneEditor : UnityEditor.Editor
    {

        public CameraCutscene Target
        {
            get { return (CameraCutscene)serializedObject.targetObject; }
        }

        public CameraCutscene TargetSingle
        {
            get { return (CameraCutscene)target; }
        }

        private ReorderableList reorderList;
        private bool listFoldout = false;

        private GameObject standardCamera;

        private void OnEnable()
        {
            reorderList = new ReorderableList(serializedObject, serializedObject.FindProperty("cutSceneData"), true, true, true, true);

            reorderList.elementHeightCallback = (_index =>
            {
                Repaint();

                var size = EditorGUIUtility.singleLineHeight * 4.0f;

                //if (Target.cutSceneData[_index].enablePositionChange)
                //    size += EditorGUIUtility.singleLineHeight * 1.25f;

                //if (Target.cutSceneData[_index].enableRotationChange)
                //    size += EditorGUIUtility.singleLineHeight * 1.25f;

                if (Target.cutSceneData[_index].enableCallback)
                {
                    size += EditorGUIUtility.singleLineHeight * 6.0f;

                    if (Target.cutSceneData[_index].onReached != null)
                    {
                        if (Target.cutSceneData[_index].onReached.GetPersistentEventCount() > 1)
                            size += (Target.cutSceneData[_index].onReached.GetPersistentEventCount() - 1) * (EditorGUIUtility.singleLineHeight * 2.75f);
                    }
                }
                else
                {
                    size += EditorGUIUtility.singleLineHeight * 1.25f;
                }

                if(Target.cutSceneData[_index].enableCustomCurve)
                {
                    size += EditorGUIUtility.singleLineHeight * 1.75f;
                }

                return size;
            });

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

                // Draw a button to set camera pos and rotation to elements values
                if (GUI.Button(new Rect(_rect.x + _rect.width - 135.0f, _rect.y + 10.0f, 60.0f, EditorGUIUtility.singleLineHeight), "Move To"))
                {
                    //if(Target.cutSceneData[_index].enablePositionChange)
                    standardCamera.transform.position = Target.cutSceneData[_index].camPosition;
                    //if (Target.cutSceneData[_index].enableRotationChange)
                    standardCamera.transform.rotation = Quaternion.Euler(Target.cutSceneData[_index].camRotation);
                }

                if (GUI.Button(new Rect(_rect.x + _rect.width - 65.0f, _rect.y + 10.0f, 60.0f, EditorGUIUtility.singleLineHeight), "Update"))
                {
                    Target.cutSceneData[_index].camPosition = standardCamera.transform.position;
                    Target.cutSceneData[_index].camRotation = standardCamera.transform.rotation.eulerAngles;
                }

                // Draw a button to update position of this element to cameras current position.
                //if (GUI.Button(new Rect(_rect.x + _rect.width - 90.0f, _rect.y + 30.0f, 20.0f, EditorGUIUtility.singleLineHeight), "P"))
                //{
                //    Target.cutSceneData[_index].camPosition = standardCamera.transform.position;
                //}

                //// Draw a button to update rotation of this element to cameras current rotation.
                //if (GUI.Button(new Rect(_rect.x + _rect.width - 70.0f, _rect.y + 30.0f, 20.0f, EditorGUIUtility.singleLineHeight), "R"))
                //{
                //    Target.cutSceneData[_index].camRotation = standardCamera.transform.rotation.eulerAngles;
                //}

                //// Draw a button to update both position and rotation of this element to cameras current values
                //if (GUI.Button(new Rect(_rect.x + _rect.width - 50.0f, _rect.y + 30.0f, 20.0f, EditorGUIUtility.singleLineHeight), "B"))
                //{
                //    Target.cutSceneData[_index].camPosition = standardCamera.transform.position;
                //    Target.cutSceneData[_index].camRotation = standardCamera.transform.rotation.eulerAngles;
                //}

                EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("haltOnReach"), GUIContent.none);
                EditorGUI.LabelField(new Rect(_rect.x + 15.0f, _rect.y, 140.0f, EditorGUIUtility.singleLineHeight), "Halt When Reached");

                _rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                // Draw check box for enabling position change, then a label after it.
                //EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight),
                //    element.FindPropertyRelative("enablePositionChange"), GUIContent.none);
                //EditorGUI.LabelField(new Rect(_rect.x + 15.0f, _rect.y, 140.0f, EditorGUIUtility.singleLineHeight), "Enable Position Change");

                //_rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                //if (Target.cutSceneData[_index].enablePositionChange)
                //{
                //    // Draw the Vector3 position field only if the enable position change checkbox is checked.
                //    EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, 200.0f,
                //        EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("camPosition"), GUIContent.none);

                //    _rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                //}

                // Draw a check box for enabling rotation change, then a label after it.
                //EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight),
                //    element.FindPropertyRelative("enableRotationChange"), GUIContent.none);
                //EditorGUI.LabelField(new Rect(_rect.x + 15.0f, _rect.y, 140.0f, EditorGUIUtility.singleLineHeight), "Enable Rotation Change");

                //_rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                //if (Target.cutSceneData[_index].enableRotationChange)
                //{
                //    EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, 200.0f,
                //        EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("camRotation"), GUIContent.none);

                //    _rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                //}

                EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("enableCustomCurve"), GUIContent.none);
                EditorGUI.LabelField(new Rect(_rect.x + 15.0f, _rect.y, 160.0f, EditorGUIUtility.singleLineHeight), "Enable Custom Curve");

                _rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                if (Target.cutSceneData[_index].enableCustomCurve)
                {
                    EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, _rect.width,
                        EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("moveCurve"), new GUIContent("Curve"));

                    _rect.y += EditorGUIUtility.singleLineHeight * 1.25f;
                }

                EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("enableCallback"), GUIContent.none);
                EditorGUI.LabelField(new Rect(_rect.x + 15.0f, _rect.y, 160.0f, EditorGUIUtility.singleLineHeight), "Enable On Reach Callback");

                _rect.y += EditorGUIUtility.singleLineHeight * 1.25f;

                if (Target.cutSceneData[_index].enableCallback)
                {
                    EditorGUI.PropertyField(new Rect(_rect.x, _rect.y, _rect.width,
                        EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("onReached"), new GUIContent("On Reached"));
                }
            };

            // Sets name in the header for the list.
            reorderList.drawHeaderCallback = (_rect) => { EditorGUI.LabelField(_rect, "Cutscene Points"); };

            EditorUtility.SetDirty(Target);
        }

        public override void OnInspectorGUI()
        {
            standardCamera = Camera.main.gameObject.transform.root.gameObject;

            serializedObject.Update();

            EditorGUILayout.LabelField("Debug Settings", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableGizmos"), true);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Cutscene Settings", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startOnInitialize"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultCurve"), true);

            if (GUILayout.Button("Move Camera to Start"))
            {
                if (Target.cutSceneData != null && Target.cutSceneData.Count > 0)
                {
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    //if (Target.cutSceneData[0].enablePositionChange)
                    standardCamera.transform.position = Target.cutSceneData[0].camPosition;
                    //if (Target.cutSceneData[0].enableRotationChange)
                    standardCamera.transform.rotation = Quaternion.Euler(Target.cutSceneData[0].camRotation);
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Cutscene Controls", EditorStyles.helpBox);

            // Button to add cameras current position to the list.
            //if (GUILayout.Button("Add Camera Position"))
            //{
            //    var cutSceneData = new CutsceneData();
            //    cutSceneData.enableRotationChange = false;
            //    cutSceneData.camPosition = standardCamera.transform.position;

            //    Target.cutSceneData.Add(cutSceneData);
            //}

            //// Button to add cameras current rotation to the list.
            //if (GUILayout.Button("Add Camera Rotation"))
            //{
            //    var cutSceneData = new CutsceneData();
            //    cutSceneData.enablePositionChange = false;
            //    cutSceneData.camRotation = standardCamera.transform.rotation.eulerAngles;

            //    Target.cutSceneData.Add(cutSceneData);
            //}

            // Button to add the current cameras position and rotation to the list.
            if (GUILayout.Button("Add New"))
            {
                var cutSceneData = new CutsceneData();
                cutSceneData.camPosition = standardCamera.transform.position;
                cutSceneData.camRotation = standardCamera.transform.rotation.eulerAngles;

                Target.cutSceneData.Add(cutSceneData);
            }

            // Button to clear the list.
            if (GUILayout.Button("Clear Cutscene"))
            {
                if (EditorUtility.DisplayDialog("Confirm Clear",
                    "Are you sure you want to clear the cutscene points? This process can not be undone.", "Yes", "No"))
                {
                    reorderList.serializedProperty.ClearArray();
                }
            }

            listFoldout = EditorGUILayout.Foldout(listFoldout, "Cutscene Points");

            if (listFoldout)
            {
                reorderList.DoLayoutList();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Cutscene State Callbacks", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onStart"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onPointReached"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onFinish"), true);

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            if (TargetSingle.cutSceneData == null) return;

            if (!TargetSingle.enableGizmos) return;

            var guiStyle = new GUIStyle();
            guiStyle.normal.textColor = Color.white;
            guiStyle.fontSize = 20;

            for (var i = 0; i < TargetSingle.cutSceneData.Count; i++)
            {
                Handles.Label(TargetSingle.cutSceneData[i].camPosition, i.ToString(), guiStyle);
            }
        }
    }
}
