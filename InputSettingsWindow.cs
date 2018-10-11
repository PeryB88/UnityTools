//=============================================================================================================================//
//
//	Project: DarkestPath
//	Created by: Pericles Barros
//  Created on: 10/10/2018 2:37:36 PM
//
//=============================================================================================================================//


#region Usings

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

#endregion

namespace InputSystem.Inspectors
{
    public class InputSettingsWindow : EditorWindow
    {
        //=====================================================================================================================//
        //================================================== Internal Classes =================================================//
        //=====================================================================================================================//

        #region Internal Classes

        private enum AxisType
        {
            KeyOrMouseButton,
            MouseMovement,
            JoystickAxis
        }

        #endregion

        //=====================================================================================================================//
        //=================================================== Private Fields ==================================================//
        //=====================================================================================================================//

        #region Private Fields

        private static SerializedObject _inputManager;
        private static SerializedProperty _inputAxesProp;
        private static ReorderableList _inputAxesList;

        private static Vector2 _scrollPos;
        private static EditorWindow _cachedWindow;

        private static bool[] OpenTabs = new bool[150];

        private static readonly string[] AxisTypeNames = {
            "Key or Mouse Button",
            "Mouse Movement",
            "Joystick Axis"
        };

        private static readonly string[] AxisNames = {
            "X Axis",
            "Y Axis",
            "3rd Axis",
            "4th Axis",
            "5th Axis",
            "6th Axis",
            "7th Axis",
            "8th Axis",
            "9th Axis",
            "10th Axis",
            "11th Axis",
            "12th Axis",
            "13th Axis",
            "14th Axis",
            "15th Axis",
            "16th Axis",
            "17th Axis",
            "18th Axis",
            "19th Axis",
            "20th Axis",
            "21st Axis",
            "22nd Axis",
            "23rd Axis",
            "24th Axis",
            "25th Axis",
            "26th Axis",
            "27th Axis",
            "28th Axis"
        };

        private static readonly string[] MouseAxesNames = {
            "X Axis",
            "Y Axis",
            "Scroll Wheel"
        };

        private static readonly string[] JoystickIndexNames = {
            "Get Motion from all Joysticks",
            "Joystick 1",
            "Joystick 2",
            "Joystick 3",
            "Joystick 4",
            "Joystick 5",
            "Joystick 6",
            "Joystick 7",
            "Joystick 8",
            "Joystick 9",
            "Joystick 10",
            "Joystick 11",
            "Joystick 12",
            "Joystick 13",
            "Joystick 14",
            "Joystick 15",
            "Joystick 16"
        };

        #endregion

        //=====================================================================================================================//
        //============================================= Unity Callback Methods ================================================//
        //=====================================================================================================================//

        #region Unity Callback Methods

        private void OnGUI()
        {
            EditorGUILayout.Space();

            if (_inputAxesList == null) {
                CreateList();
            }

            if (_inputAxesList == null)
                return;

            if (_cachedWindow == null) {
                OpenWindow();
                return;
            }

            _inputManager.Update();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MaxWidth(_cachedWindow.position.width), GUILayout.MaxHeight(_cachedWindow.position.height));
            _inputAxesList.DoLayoutList();
            EditorGUILayout.EndScrollView();

            _inputManager.ApplyModifiedProperties();
        }

        #endregion

        //=====================================================================================================================//
        //================================================== Private Methods ==================================================//
        //=====================================================================================================================//

        #region Private Methods

        [MenuItem("Tools/Input Settings")]
        private static void OpenWindow()
        {
            _cachedWindow = GetWindow(typeof(InputSettingsWindow));
            _cachedWindow.titleContent = new GUIContent("Input Settings");
            _cachedWindow.Show();

            CreateList();
        }

        private static void CreateList()
        {
            _inputManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            _inputAxesProp = _inputManager.FindProperty("m_Axes");

            _inputAxesList = new ReorderableList(_inputManager, _inputAxesProp, true, true, true, true) {
                headerHeight = EditorGUIUtility.singleLineHeight + 3f,
                elementHeightCallback = CalculateElementHeight,
                drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "Input Axes"); },
                drawElementCallback = DrawAxisElement,
                onRemoveCallback = (list) => {
                    OpenTabs[list.index] = false;
                    ReorderableList.defaultBehaviours.DoRemoveButton(_inputAxesList);
                }
            };
        }

        //TODO: Add tooltips
        private static void DrawAxisElement(Rect rect, int index, bool selected, bool focused)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            if (index < 0 || index >= _inputAxesList.count)
                return;

            var inputAxisProp = _inputAxesProp.GetArrayElementAtIndex(index);
            if (inputAxisProp == null)
                return;

            var axisName = inputAxisProp.FindPropertyRelative("m_Name");

            var labelRect = rect;
            labelRect.x += 15f;

            var foldRect = rect;
            foldRect.width = EditorGUIUtility.singleLineHeight + 2f;
            foldRect.height = EditorGUIUtility.singleLineHeight + 3f;

            if (GUI.Button(foldRect, "",
                OpenTabs[index] ? GUI.skin.FindStyle("IN Foldout") : GUI.skin.FindStyle("Foldout"))) {
                OpenTabs[index] = !OpenTabs[index];
            }

            EditorGUI.LabelField(labelRect, axisName.stringValue);

            if (!OpenTabs[index])
                return;

            ++EditorGUI.indentLevel;
            EditorGUIUtility.labelWidth = 150f;

            //----------------------------- Name
            var nameRect = rect;
            nameRect.width -= 15f;
            nameRect.height = EditorGUIUtility.singleLineHeight;
            nameRect.y += EditorGUIUtility.singleLineHeight + 3f;

            EditorGUI.PropertyField(nameRect, axisName, new GUIContent("Axis Name"));

            //----------------------------- Type
            var typeProp = inputAxisProp.FindPropertyRelative("type");
            var typeRect = nameRect;
            typeRect.height = EditorGUIUtility.singleLineHeight;
            typeRect.y += EditorGUIUtility.singleLineHeight + 3f;

            typeProp.intValue = EditorGUI.Popup(typeRect, "Type", typeProp.intValue, AxisTypeNames);

            var type = (AxisType) typeProp.intValue;

            var nextRect = typeRect;
            nextRect.y += EditorGUIUtility.singleLineHeight + 3f;
            if (type == AxisType.KeyOrMouseButton) {
                //----------------------------- Positive Button
                var positiveBtnProp = inputAxisProp.FindPropertyRelative("positiveButton");
                EditorGUI.PropertyField(nextRect, positiveBtnProp, new GUIContent("Positive Button"));
                nextRect.y += EditorGUIUtility.singleLineHeight + 3f;

                //----------------------------- Alternate Positive Button
                var altPositiveBtnProp = inputAxisProp.FindPropertyRelative("altPositiveButton");
                EditorGUI.PropertyField(nextRect, altPositiveBtnProp, new GUIContent("Alt Positive Btn"));
                nextRect.y += EditorGUIUtility.singleLineHeight + 3f;

                //----------------------------- Positive Description
                var positiveDescProp = inputAxisProp.FindPropertyRelative("descriptiveName");
                EditorGUI.PropertyField(nextRect, positiveDescProp, new GUIContent("Positive Btn Desc"));
                nextRect.y += EditorGUIUtility.singleLineHeight + 3f;

                //----------------------------- Negative Button
                var negativeBtnProp = inputAxisProp.FindPropertyRelative("negativeButton");
                EditorGUI.PropertyField(nextRect, negativeBtnProp, new GUIContent("Negative Button"));
                nextRect.y += EditorGUIUtility.singleLineHeight + 3f;

                //----------------------------- Alternate Negative Buttons
                var altNegativeBtnProp = inputAxisProp.FindPropertyRelative("altNegativeButton");
                EditorGUI.PropertyField(nextRect, altNegativeBtnProp, new GUIContent("Alt Negative Btn"));
                nextRect.y += EditorGUIUtility.singleLineHeight + 3f;

                //----------------------------- Negative Description
                var negativeDescProp = inputAxisProp.FindPropertyRelative("descriptiveNegativeName");
                EditorGUI.PropertyField(nextRect, negativeDescProp, new GUIContent("Negative Btn Desc"));
                nextRect.y += EditorGUIUtility.singleLineHeight + 3f;
            } else {
                //----------------------------- Axis
                var axisProp = inputAxisProp.FindPropertyRelative("axis");
                if (type == AxisType.JoystickAxis) {
                    axisProp.intValue = EditorGUI.Popup(nextRect, "Axis", axisProp.intValue, AxisNames);
                    nextRect.y += EditorGUIUtility.singleLineHeight + 3f;

                    //----------------------------- Joystick Number
                    var joyNumProp = inputAxisProp.FindPropertyRelative("joyNum");
                    joyNumProp.intValue = EditorGUI.Popup(nextRect, "Joystick Index", joyNumProp.intValue,
                        JoystickIndexNames);
                } else {
                    axisProp.intValue = EditorGUI.Popup(nextRect, "Axis", Mathf.Clamp(axisProp.intValue, 0, 2),
                        MouseAxesNames);
                }

                nextRect.y += EditorGUIUtility.singleLineHeight + 3f;
            }

            //----------------------------- Gravity
            var gravityProp = inputAxisProp.FindPropertyRelative("gravity");
            EditorGUI.PropertyField(nextRect, gravityProp, new GUIContent("Gravity"));
            nextRect.y += EditorGUIUtility.singleLineHeight + 3f;

            //----------------------------- Deadzone
            var deadzoneProp = inputAxisProp.FindPropertyRelative("dead");
            EditorGUI.PropertyField(nextRect, deadzoneProp, new GUIContent("Deadzone"));
            nextRect.y += EditorGUIUtility.singleLineHeight + 3f;

            //----------------------------- Sensitivity
            var sensitivityProp = inputAxisProp.FindPropertyRelative("sensitivity");
            EditorGUI.PropertyField(nextRect, sensitivityProp, new GUIContent("Sensitivity"));
            nextRect.y += EditorGUIUtility.singleLineHeight + 3f;

            //----------------------------- Snap
            var snapProp = inputAxisProp.FindPropertyRelative("snap");
            EditorGUI.PropertyField(nextRect, snapProp, new GUIContent("Snap"));
            nextRect.y += EditorGUIUtility.singleLineHeight + 3f;

            //----------------------------- Inverted
            var invertedProp = inputAxisProp.FindPropertyRelative("invert");
            EditorGUI.PropertyField(nextRect, invertedProp, new GUIContent("Inverted"));

            //Draw axis data
            EditorGUIUtility.labelWidth = labelWidth;
            --EditorGUI.indentLevel;
        }

        private static float CalculateElementHeight(int index)
        {
            if (!OpenTabs[index])
                return EditorGUIUtility.singleLineHeight + 3f;

            var inputAxisProp = _inputAxesProp.GetArrayElementAtIndex(index);
            var typeProp = inputAxisProp.FindPropertyRelative("type");

            switch ((AxisType) typeProp.intValue) {
                case AxisType.JoystickAxis:
                    return (EditorGUIUtility.singleLineHeight + 3f) * 10f + 3f;
                case AxisType.MouseMovement:
                    return (EditorGUIUtility.singleLineHeight + 3f) * 9f + 3f;
            }

            return (EditorGUIUtility.singleLineHeight + 3f) * 14 + 3f;
        }

        #endregion
    }
}