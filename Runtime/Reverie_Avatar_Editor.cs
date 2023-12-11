using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


namespace ReverieSDK
{
    [CustomEditor(typeof(Reverie_Avatar))]
    public class Reverie_Avatar_Editor : Editor
    {
        private bool _editEyePos = false;
        private bool _editMouthPos = false;
        private bool _editEyeConstraint = false;
        private bool _visemeFoldout = false;
        private GUIStyle selctionLabelStyle;
        
        private bool requirementsNotMet;
        
        public override void OnInspectorGUI()
        {
            selctionLabelStyle = EditorStyles.label;
            selctionLabelStyle.richText = true;
            
            Reverie_Avatar avatar = (Reverie_Avatar)target;

            GetErrors(avatar);
            
            EditorGUI.BeginDisabledGroup(requirementsNotMet);

            GUILayout.Space(10f);

            using (new GUILayout.VerticalScope("box"))
            {
                AreaTitle("Avatar Settings");
                
                GUILayout.Space(10f);
                
                using (new GUILayout.HorizontalScope())
                {
                    CenterLabelVertical("Face Mesh:");

                    EditorGUI.BeginChangeCheck();
                    avatar.faceMesh = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
                        avatar.faceMesh,
                        typeof(SkinnedMeshRenderer),
                        true,
                        GUILayout.Height(25f), GUILayout.Width(300)
                    );
                    if (EditorGUI.EndChangeCheck()) avatar.GetFaceInfo();
                }
                
                using (new GUILayout.HorizontalScope())
                {
                    CenterLabelVertical("Animator Controller:");

                    avatar.avatarController = (RuntimeAnimatorController)EditorGUILayout.ObjectField(
                        avatar.avatarController,
                        typeof(RuntimeAnimatorController),
                        true, GUILayout.Height(25f), GUILayout.Width(300));
                }

                GUILayout.Space(10f);
                
                // Eye Settings
                using (new GUILayout.VerticalScope("box"))
                {
                    AreaTitle("Eye Settings");
                    GUILayout.Space(10f);

                    using (new GUILayout.HorizontalScope())
                    {
                        if (_editEyePos) GUI.color = Color.green;
                        if (GUILayout.Button("Edit Eye Position"))
                        {
                            _editMouthPos = false;
                            _editEyePos = !_editEyePos;
                            SceneView.RepaintAll();
                        }

                        GUI.color = Color.white;
                        EditorGUI.BeginChangeCheck();
                        avatar.eyePositon = EditorGUILayout.Vector3Field("", avatar.eyePositon);
                        if (EditorGUI.EndChangeCheck())
                        {
                            SceneView.RepaintAll();
                            Undo.RegisterCompleteObjectUndo(avatar, "Change Mouth Position");
                        }
                    }

                    GUILayout.Space(10f);

                    EditorGUI.BeginDisabledGroup(!avatar.usingEyes);
                    using (new GUILayout.HorizontalScope())
                    {
                        CenterLabelVertical("Left Eye:", 60f);
                        avatar.leftEye =
                            EditorGUILayout.ObjectField(
                                avatar.leftEye,
                                typeof(Transform),
                                true,
                                GUILayout.Height(25f)) as Transform;

                        GUILayout.FlexibleSpace();
                        GUILayout.Space(10f);

                        CenterLabelVertical("Right Eye:", 70f);
                        avatar.rightEye =
                            EditorGUILayout.ObjectField(
                                avatar.rightEye,
                                typeof(Transform),
                                true,
                                GUILayout.Height(25f)) as Transform;
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        var buttonStyle = new GUIStyle(GUI.skin.button);
                        if (_editEyeConstraint) GUI.color = Color.green;
                        if (GUILayout.Button("Edit Eye Constraints", buttonStyle, GUILayout.Height(20f)))
                        {
                            _editEyeConstraint = !_editEyeConstraint;
                            if (!_editEyeConstraint)
                            {
                                if (EditorGUI.EndChangeCheck()) avatar.PreviewAngleLimit(Reverie_Avatar.EyeConstraintDirection.None);
                            }
                        }

                        GUI.color = Color.white;
                    }

                    if (_editEyeConstraint)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("Up", GUILayout.Width(50f));
                            EditorGUI.BeginChangeCheck();
                            avatar.angleLimitUp = EditorGUILayout.Slider(avatar.angleLimitUp, 0f, 90f);
                            if (EditorGUI.EndChangeCheck() && _editEyeConstraint)
                                avatar.PreviewAngleLimit(Reverie_Avatar.EyeConstraintDirection.Up);
                        }

                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("Down", GUILayout.Width(50f));
                            EditorGUI.BeginChangeCheck();
                            avatar.angleLimitDown = EditorGUILayout.Slider(avatar.angleLimitDown, 0f, 90f);
                            if (EditorGUI.EndChangeCheck() && _editEyeConstraint)
                                avatar.PreviewAngleLimit(Reverie_Avatar.EyeConstraintDirection.Down);
                        }

                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("Left", GUILayout.Width(50f));
                            EditorGUI.BeginChangeCheck();
                            avatar.angleLimitLeft = EditorGUILayout.Slider(avatar.angleLimitLeft, 0f, 90f);
                            if (EditorGUI.EndChangeCheck() && _editEyeConstraint)
                                avatar.PreviewAngleLimit(Reverie_Avatar.EyeConstraintDirection.Left);
                        }

                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("Right", GUILayout.Width(50f));
                            EditorGUI.BeginChangeCheck();
                            avatar.angleLimitRight = EditorGUILayout.Slider(avatar.angleLimitRight, 0f, 90f);
                            if (EditorGUI.EndChangeCheck() && _editEyeConstraint)
                                avatar.PreviewAngleLimit(Reverie_Avatar.EyeConstraintDirection.Right);
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                    
                    GUILayout.Space(10f);

                    using (new GUILayout.HorizontalScope())
                    {
                        CenterLabelVertical("Left Blink:", 60f);
                        avatar.leftEyeBlinkIndex = EditorGUILayout.Popup(
                            avatar.leftEyeBlinkIndex,
                            avatar.faceMeshBlendshapeNames.ToArray(),
                            GUILayout.Height(25f));

                        GUILayout.FlexibleSpace();
                        GUILayout.Space(10f);

                        CenterLabelVertical("Right Blink:", 70f);
                        avatar.rightEyeBlinkIndex = EditorGUILayout.Popup(
                            avatar.rightEyeBlinkIndex,
                            avatar.faceMeshBlendshapeNames.ToArray(),
                            GUILayout.Height(25f));

                    }
                }

                GUILayout.Space(10f);

                // Mouth Settings
                using (new GUILayout.VerticalScope("box"))
                {
                    AreaTitle("Mouth Settings");
                    GUILayout.Space(10f);

                    using (new GUILayout.HorizontalScope())
                    {
                        var buttonStyle = new GUIStyle(GUI.skin.button);
                        if (_editMouthPos) GUI.color = Color.green;
                        if (GUILayout.Button("Edit Mouth Position", buttonStyle))
                        {
                            _editEyePos = false;
                            _editMouthPos = !_editMouthPos;
                            SceneView.RepaintAll();
                        }

                        GUI.color = Color.white;
                        EditorGUI.BeginChangeCheck();
                        avatar.mouthPositon = EditorGUILayout.Vector3Field("", avatar.mouthPositon);
                        if (EditorGUI.EndChangeCheck())
                        {
                            SceneView.RepaintAll();
                            Undo.RegisterCompleteObjectUndo(avatar, "Change Mouth Position");
                        }
                    }

                    if (avatar.faceMesh != null)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Space(10);
                            _visemeFoldout = EditorGUILayout.Foldout(_visemeFoldout, "Visemes");

                            if (GUILayout.Button("Auto Fill Visemes", GUILayout.Height(20f)))
                            {
                                avatar.AutoFillVisemes();
                            }
                        }

                        if (_visemeFoldout)
                        {
                            for (int i = 0; i < Enum.GetNames(typeof(Reverie_Avatar._visemes)).Length; i++)
                            {
                                avatar.visemeList[i] = EditorGUILayout.Popup(Enum.GetNames(typeof(Reverie_Avatar._visemes))[i],
                                    avatar.visemeList[i],
                                    avatar.faceMeshBlendshapeNames.ToArray());
                            }
                        }
                    }

                    serializedObject.Update();
                }
            }

            GUILayout.Space(10f);
            
            // Build Avatar
            using (new GUILayout.VerticalScope("box"))
            {
                AreaTitle("Export Avatar");
                
                avatar.avatarBundleName = EditorGUILayout.TextField("Avatar Bundle Name", avatar.avatarBundleName);
                
                using (var horizontalScope = new GUILayout.HorizontalScope("box"))
                {
                    var btnstyle = GUI.skin.button;
                    btnstyle.richText = true;
                    if (GUILayout.Button($"<size=14><b><color=white>Build and Save</color></b></size>",
                            GUILayout.Height(40f)))
                    {
                        avatar.BuildAvatar();
                    }
                }
            }
            
            EditorGUI.EndDisabledGroup();
        }

        private void GetErrors(Reverie_Avatar avatar)
        {
            if (avatar.avatarAnimator == null)
            {
                avatar.avatarAnimator = avatar.GetComponent<Animator>();
                if (avatar.avatarAnimator == null)
                {
                    EditorGUILayout.HelpBox("Avatar must have an Animator component attached", MessageType.Error);
                    requirementsNotMet = true;
                    return;
                }
            }
            if (avatar.avatarAnimator.avatar == null || avatar.avatarAnimator.avatar.isHuman == false)
            {
                EditorGUILayout.HelpBox("Please assign a valid Humanoid Avatar Definition to the Animator.", MessageType.Error);
                requirementsNotMet = true;
                return;
            }
            if (avatar.avatarAnimator.GetBoneTransform(HumanBodyBones.LeftToes) == null || avatar.avatarAnimator.GetBoneTransform(HumanBodyBones.RightToes) == null)
            {
                EditorGUILayout.HelpBox("Please assign toe bones to the Avatar Humanoid Description", MessageType.Error);
                requirementsNotMet = true;
                return;
            }
            
            if (requirementsNotMet) avatar.GetDefaultValues();
            
            requirementsNotMet = false;
            
            GetWarnings(avatar);
        }
        private void GetWarnings(Reverie_Avatar avatar)
        {
            if (avatar.leftEye == null || avatar.rightEye == null)
            {
                EditorGUILayout.HelpBox("Your Avatar does not have eye bones assigned to your Humanoid Description. Is this what you want?", MessageType.Warning);
            }
            else if (avatar.leftEye != null && avatar.rightEye != null)
            {
                if (avatar.leftEye.up.y < 0.98f || avatar.rightEye.up.y < 0.98f)
                {
                    EditorGUILayout.HelpBox("Your eye bones are not oriented with Y-Up. This will cause problems for eye movement.", MessageType.Warning);
                }
            }
        }

        private void AreaTitle(string title)
        {
            using (var horizontalScope = new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label($"<size=14><b><color=white>{title}</color></b></size>", selctionLabelStyle);
                GUILayout.FlexibleSpace();
            }
        }

        private void CenterLabelVertical(string text, float width = 0)
        {
            using (var areaScope = new GUILayout.VerticalScope())
            {
                GUILayout.FlexibleSpace();
                if (width > 0) GUILayout.Label(text, GUILayout.Width(width));
                else GUILayout.Label(text);
                GUILayout.FlexibleSpace();
            }
        }
        
        protected virtual void OnSceneGUI()
        {
            Reverie_Avatar avatar = (Reverie_Avatar)target;

            if (_editEyePos)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition =
                    Handles.PositionHandle(avatar.eyePositon + avatar.transform.position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(avatar, "Change Eye Position");
                    avatar.eyePositon = newTargetPosition - avatar.transform.position;
                }
            }
            
            if (_editMouthPos)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition =
                    Handles.PositionHandle(avatar.mouthPositon + avatar.transform.position, avatar.transform.rotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(avatar, "Change Mouth Position");
                    avatar.mouthPositon = newTargetPosition - avatar.transform.position;
                }
            }
        }
    }
}
#endif