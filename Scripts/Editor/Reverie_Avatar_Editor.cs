using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;


namespace ReverieSDK
{
    [CustomEditor(typeof(Reverie_Avatar))]
    public class Reverie_Avatar_Editor : Editor
    {
        public bool togglesFoldout = false;
        public Reverie_Avatar avatar;
        
        private bool editEyePos = false;
        private bool editMouthPos = false;
        private bool editEyeConstraint = false;
        private bool visemeFoldout = false;
        private GUIStyle selctionLabelStyle;
        private bool requirementsNotMet;
        private Vector2 animationScrollPos = Vector2.zero;
        
        private void OnEnable()
        {
            avatar = (Reverie_Avatar)target;
        }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                base.OnInspectorGUI();
                return;
            };
            
            selctionLabelStyle = EditorStyles.label;
            selctionLabelStyle.richText = true;
            
            CheckForErrors(avatar);
            
            EditorGUI.BeginDisabledGroup(requirementsNotMet);

            GUILayout.Space(10f);

            EditorGUI.BeginChangeCheck();
            
            using (new GUILayout.VerticalScope("box"))
            {
                AreaTitle("Avatar Settings");
                
                GUILayout.Space(10f);
                
                using (new GUILayout.HorizontalScope())
                {
                    CenterLabelVertical("Face Mesh:", 0f, "Mesh that contain the face blendshapes");

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
                    CenterLabelVertical("Animator Controller:", 0f, "Animator Controller to apply over top of normal reverie animations");

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
                        if (editEyePos) GUI.color = Color.green;
                        if (GUILayout.Button(new GUIContent("Edit Eye Position", "This is where the virtual camera will be placed" +
                            "for your view. It is recommended to place this roughly between the eyes."), GUILayout.Height(20f)))
                        {
                            editMouthPos = false;
                            editEyePos = !editEyePos;
                            SceneView.RepaintAll();
                        }

                        GUI.color = Color.white;
                        EditorGUI.BeginChangeCheck();
                        avatar.eyePositon = EditorGUILayout.Vector3Field("", avatar.eyePositon);
                        if (EditorGUI.EndChangeCheck())
                        {
                            SceneView.RepaintAll();
                            Undo.RegisterCompleteObjectUndo(avatar, "Change Eye Position");
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
                        if (editEyeConstraint) GUI.color = Color.green;
                        if (GUILayout.Button("Edit Eye Constraints", buttonStyle, GUILayout.Height(20f)))
                        {
                            editEyeConstraint = !editEyeConstraint;
                            if (!editEyeConstraint)
                            {
                                if (EditorGUI.EndChangeCheck()) avatar.PreviewAngleLimit(Reverie_Avatar.EyeConstraintDirection.None);
                            }
                        }

                        GUI.color = Color.white;
                    }

                    if (editEyeConstraint)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("Up", GUILayout.Width(50f));
                            EditorGUI.BeginChangeCheck();
                            avatar.angleLimitUp = EditorGUILayout.Slider(avatar.angleLimitUp, 0f, 90f);
                            if (EditorGUI.EndChangeCheck() && editEyeConstraint)
                                avatar.PreviewAngleLimit(Reverie_Avatar.EyeConstraintDirection.Up);
                        }

                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("Down", GUILayout.Width(50f));
                            EditorGUI.BeginChangeCheck();
                            avatar.angleLimitDown = EditorGUILayout.Slider(avatar.angleLimitDown, 0f, 90f);
                            if (EditorGUI.EndChangeCheck() && editEyeConstraint)
                                avatar.PreviewAngleLimit(Reverie_Avatar.EyeConstraintDirection.Down);
                        }

                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("Left", GUILayout.Width(50f));
                            EditorGUI.BeginChangeCheck();
                            avatar.angleLimitLeft = EditorGUILayout.Slider(avatar.angleLimitLeft, 0f, 90f);
                            if (EditorGUI.EndChangeCheck() && editEyeConstraint)
                                avatar.PreviewAngleLimit(Reverie_Avatar.EyeConstraintDirection.Left);
                        }

                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Space(10);
                            GUILayout.Label("Right", GUILayout.Width(50f));
                            EditorGUI.BeginChangeCheck();
                            avatar.angleLimitRight = EditorGUILayout.Slider(avatar.angleLimitRight, 0f, 90f);
                            if (EditorGUI.EndChangeCheck() && editEyeConstraint)
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
                        if (editMouthPos) GUI.color = Color.green;
                        if (GUILayout.Button(new GUIContent("Edit Mouth Position", "This is where your speech will be transmitted from."),
                            buttonStyle, GUILayout.Height(20f)))
                        {
                            editEyePos = false;
                            editMouthPos = !editMouthPos;
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
                            visemeFoldout = EditorGUILayout.Foldout(visemeFoldout, new GUIContent("Visemes", "These are the" +
                                " blendshapes that will be used for auto lip tracking using your microphone."));
                            if (GUILayout.Button("Auto Fill Visemes", GUILayout.Height(20f)))
                            {
                                avatar.AutoFillVisemes();
                            }
                        }

                        if (visemeFoldout)
                        {
                            for (int i = 0; i < Enum.GetNames(typeof(Reverie_Avatar._visemes)).Length; i++)
                            {
                                avatar.visemeList[i] = EditorGUILayout.Popup(Enum.GetNames(typeof(Reverie_Avatar._visemes))[i],
                                    avatar.visemeList[i],
                                    avatar.faceMeshBlendshapeNames.ToArray());
                            }
                        }
                    }
                }
                
                GUILayout.Space(10f);
                
                // Toggle Animations
                animationScrollPos = EditorGUILayout.BeginScrollView(animationScrollPos, GUILayout.ExpandHeight(true));
                ReverieAvatarToggleGroups_Editor.GroupList(this);
                EditorGUILayout.EndScrollView();
            }

            GUILayout.Space(10f);
            
            // Build Avatar
            using (new GUILayout.VerticalScope("box"))
            {
                AreaTitle("Export Avatar");
                
                avatar.avatarBundleName = EditorGUILayout.TextField("Avatar Bundle Name", avatar.avatarBundleName);
                
                using (new GUILayout.HorizontalScope("box"))
                {
                    var btnstyle = GUI.skin.button;
                    btnstyle.richText = true;
                    if (GUILayout.Button($"<size=14><b><color=white>Build and Save</color></b></size>",
                            GUILayout.Height(40f)))
                    {
                        avatar.BuildAvatar();
                        GUIUtility.ExitGUI();
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck()) EditorSceneManager.MarkSceneDirty(avatar.gameObject.scene);
            serializedObject.Update();

        }

        private void CheckForErrors(Reverie_Avatar avatar)
        {
            bool isWarning = false;
            
            if (avatar.avatarAnimator == null)
            {
                avatar.avatarAnimator = avatar.GetComponent<Animator>();
                if (avatar.avatarAnimator == null)
                {
                    EditorGUILayout.HelpBox("Avatar must have an Animator component attached", MessageType.Error);
                    isWarning = true;
                }
            }
            if (avatar.avatarAnimator.avatar == null || avatar.avatarAnimator.avatar.isHuman == false)
            {
                EditorGUILayout.HelpBox("Please assign a valid Humanoid Avatar Definition to the Animator.", MessageType.Error);
                isWarning = true;
            }
            if (avatar.avatarAnimator.GetBoneTransform(HumanBodyBones.LeftToes) == null || avatar.avatarAnimator.GetBoneTransform(HumanBodyBones.RightToes) == null)
            {
                EditorGUILayout.HelpBox("Please assign toe bones to the Avatar Humanoid Description", MessageType.Error);
                isWarning = true;
            }

            // Check model import settings
            string path = AssetDatabase.GetAssetPath(this.avatar.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            importer.isReadable = true;
 
            //Force legacy blendshape normals
            string pName = "legacyComputeAllNormalsFromSmoothingGroupsWhenMeshHasBlendShapes";
            PropertyInfo prop = importer.GetType().GetProperty(pName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if ((bool)prop.GetValue(importer) == false)
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.HelpBox("Please enable LegacyBlendShapeNormals on model import settings.", MessageType.Error);
                    if (GUILayout.Button("Fix", GUILayout.Width(100), GUILayout.ExpandHeight(true)))
                    {
                        prop.SetValue(importer, true);
                        importer.SaveAndReimport();
                        avatar.GetDefaultValues();
                    }
                }
                isWarning = true;
            }
            
            requirementsNotMet = isWarning;
            
            GetWarnings(avatar);
        }
        private void GetWarnings(Reverie_Avatar avatar)
        {
            if (avatar.leftEye == null || avatar.rightEye == null)
            {
                EditorGUILayout.HelpBox("Your Avatar does not have eye bones assigned to your Humanoid Description. Is this what you want?", MessageType.Warning);
            }
            else if (avatar.leftEye != null && avatar.rightEye != null && !editEyeConstraint)
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

        private void CenterLabelVertical(string text, float width = 0, string tooltip = "")
        {
            using (var areaScope = new GUILayout.VerticalScope())
            {
                GUILayout.FlexibleSpace();
                if (width > 0) GUILayout.Label(new GUIContent(text, tooltip), GUILayout.Width(width));
                else GUILayout.Label(new GUIContent(text, tooltip));
                GUILayout.FlexibleSpace();
            }
        }
        
        protected virtual void OnSceneGUI()
        {
            Reverie_Avatar avatar = (Reverie_Avatar)target;

            Vector3 posOffset = new Vector3(avatar.transform.position.x, 0, avatar.transform.position.z);
            
            if (editEyePos)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition =
                    Handles.PositionHandle(avatar.eyePositon + posOffset, avatar.transform.rotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(avatar, "Change Eye Position");
                    avatar.eyePositon = newTargetPosition - posOffset;
                }
            }
            
            if (editMouthPos)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition =
                    Handles.PositionHandle(avatar.mouthPositon + posOffset, avatar.transform.rotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(avatar, "Change Mouth Position");
                    avatar.mouthPositon = newTargetPosition - posOffset;
                }
            }
        }
    }
}
#endif