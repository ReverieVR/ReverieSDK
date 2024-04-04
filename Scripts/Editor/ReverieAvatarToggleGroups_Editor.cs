using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ReverieSDK
{
    public class ReverieAvatarToggleGroups_Editor
    {
        private static Texture2D plusIcon;
        private static Texture2D minusIcon;
        private static Texture2D buttontex;
        private static Vector2 scrollPos;
        public static Texture2D red;
        public static Texture2D darkred;
        public static Texture2D green;
        public static Texture2D darkgreen;
        

        public static void GroupList(Reverie_Avatar_Editor avatarEditor)
        {
            var avatarToggles = avatarEditor.avatar.avatarToggles;
            
            plusIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ReverieSDK/Assets/plus.png", typeof(Texture2D));
            minusIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ReverieSDK/Assets/minus.png", typeof(Texture2D));
            
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(10);
                
                GUIStyle selctionLabelStyle = EditorStyles.label;
                selctionLabelStyle.richText = true;
                
                EditorGUILayout.LabelField(
                    new GUIContent($"<size=14><b><color=white>Toggle Animations</color></b></size>", 
                    "Toggle Animations to enable and disable while in-game."),
                    GUILayout.Width(150)
                );
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(minusIcon, GUILayout.Width(25), GUILayout.Height(25)))
                {
                    if (avatarToggles.Count <= 0) { return; }

                    avatarToggles.RemoveAt(avatarToggles.Count - 1);
                }

                if (GUILayout.Button(plusIcon, GUILayout.Width(25), GUILayout.Height(25)))
                {
                    avatarToggles.Add(new Reverie_Avatar.AvatarToggle());
                }
            }
            
            GUIStyle nameStyle = new GUIStyle("textfield")
            {
                fontSize = 14,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter
            };

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
            for (int i = 0; i < avatarToggles.Count; i++)
            {
                GUIStyle style = new GUIStyle("window")
                {
                    fontStyle = FontStyle.Bold,
                    margin = new RectOffset(5, 5, 5, 5)
                };
                GUILayout.BeginVertical(style, GUILayout.ExpandHeight(true));
                GUILayout.Space(-12);
                GUILayout.BeginHorizontal();
                GUILayout.Space(8);
                avatarToggles[i].toggleName = GUILayout.TextField(
                    avatarToggles[i].toggleName,
                    nameStyle,
                    GUILayout.ExpandWidth(true),
                    GUILayout.MinWidth(120),
                    GUILayout.ExpandWidth(false)
                );

                GUILayout.FlexibleSpace();

                GUILayout.Space(8);

                GUILayout.EndHorizontal();
                GameObjectList(
                    ref avatarToggles[i].toggleObjectCount,
                    ref avatarToggles[i].toggleObject,
                    ref avatarToggles[i].objectOffStates,
                    ref avatarToggles[i].objectOnStates
                );
                ShapekeyList(
                    ref avatarToggles[i].toggleShapekeyCount,
                    ref avatarToggles[i].shapekeyMesh,
                    ref avatarToggles[i].shapekeyIndex,
                    ref avatarToggles[i].shapekeyName,
                    ref avatarToggles[i].shapekeyOffStates,
                    ref avatarToggles[i].shapekeyOnStates
                );
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndScrollView();
            
            avatarEditor.avatar.avatarToggles = avatarToggles;
        }

        static void GameObjectList(ref int count, ref List<GameObject> objects, ref List<bool> onStates, ref List<bool> offStates)
        {
            GUIStyle layout = new GUIStyle("window") { margin = new RectOffset(10, 10, 10, 10) };
            GUILayout.BeginVertical(GUIContent.none, layout, GUILayout.ExpandHeight(true));
            GUILayout.Space(-18);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(minusIcon, GUILayout.Width(20), GUILayout.Height(20)))
            {
                if (count <= 0) { return; }

                count--;
                objects.RemoveAt(objects.Count - 1);
                offStates.RemoveAt(offStates.Count - 1);
                onStates.RemoveAt(onStates.Count - 1);
            }

            if (GUILayout.Button(plusIcon, GUILayout.Width(20), GUILayout.Height(20)))
            {
                count++;
                objects.Add(null);
                offStates.Add(false);
                onStates.Add(true);
            }

            GUILayout.Label("GameObjects");
            GUILayout.EndHorizontal();

            for (int i = 0; i < count; i++)
            {
                GUILayout.BeginHorizontal();
                objects[i] = (GameObject)EditorGUILayout.ObjectField(
                    objects[i],
                    typeof(GameObject),
                    true,
                    GUILayout.Height(20f),
                    GUILayout.Width(150f)
                );
                
                
                GUILayout.Space(10);
                
                GUILayout.Label("Off State:");
                offStates[i] = GUILayout.Toggle(offStates[i], "");
                
                GUILayout.Space(10);
                
                GUILayout.Label("On State:");
                onStates[i] = GUILayout.Toggle(onStates[i], "");
                
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            
        }

        static void ShapekeyList(ref int count, ref List<SkinnedMeshRenderer> mesh, ref List<int> index, ref List<string> shapekey,
            ref List<float> onStates, ref List<float> offStates)
        {
            GUIStyle layout = new GUIStyle("window") { margin = new RectOffset(10, 10, 10, 10) };
            GUILayout.BeginVertical(GUIContent.none, layout, GUILayout.ExpandHeight(true));
            GUILayout.Space(-18);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(minusIcon, GUILayout.Width(20), GUILayout.Height(20)))
            {
                if (count <= 0) { return; }

                count--;
                shapekey.RemoveAt(shapekey.Count - 1);
                mesh.RemoveAt(mesh.Count - 1);
                index.RemoveAt(index.Count - 1);
                offStates.RemoveAt(offStates.Count - 1);
                onStates.RemoveAt(onStates.Count - 1);
            }

            if (GUILayout.Button(plusIcon, GUILayout.Width(20), GUILayout.Height(20)))
            {
                count++;
                shapekey.Add(null);
                mesh.Add(null);
                index.Add(0);
                offStates.Add(0);
                onStates.Add(100);
            }

            GUILayout.Label("Blendshapes");
            GUILayout.EndHorizontal();

            for (int i = 0; i < count; i++)
            {
                GUILayout.BeginHorizontal();
                mesh[i] = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
                    mesh[i],
                    typeof(SkinnedMeshRenderer),
                    true,
                    GUILayout.Height(20f),
                    GUILayout.Width(150f)
                );
                if (mesh[i] != null)
                {
                    if (mesh[i].sharedMesh.blendShapeCount > 0)
                    {
                        index[i] = EditorGUILayout.Popup(index[i], GetShapekeys(mesh[i]),GUILayout.Width(80));
                        shapekey[i] = mesh[i].sharedMesh.GetBlendShapeName(index[i]);
                        GUILayout.Space(5);
                        
                        GUILayout.Label("Off State:");
                        offStates[i] = EditorGUILayout.Slider(offStates[i], 0, 100);
                
                        GUILayout.Space(10);
                
                        GUILayout.Label("On State:");
                        onStates[i] = EditorGUILayout.Slider(onStates[i], 0, 100);
                        
                        GUILayout.FlexibleSpace();
                    }
                    else
                    {
                        GUILayout.Label("No Blendshapes");
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            
            
        }

        static string[] GetShapekeys(SkinnedMeshRenderer renderer)
        {
            List<string> shapekeys = new List<string>();

            for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
            {
                shapekeys.Add(renderer.sharedMesh.GetBlendShapeName(i));
            }

            return shapekeys.ToArray();
        }
    }
}