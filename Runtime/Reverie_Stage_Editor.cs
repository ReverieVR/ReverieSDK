using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace ReverieSDK
{
    [CustomEditor(typeof(Reverie_Stage))]
    public class Reverie_Stage_Editor : Editor
    {
        private bool _editSpawnPos;
        private GUIStyle selctionLabelStyle;
        
        public override void OnInspectorGUI()
        {
            selctionLabelStyle = EditorStyles.label;
            selctionLabelStyle.richText = true;
            
            Reverie_Stage stage = (Reverie_Stage)target;
            
            // Stage Settings
            using (new GUILayout.VerticalScope("box"))
            {
                AreaTitle("Stage Spawn");
                
                using (var horizontalScope = new GUILayout.HorizontalScope())
                {
                    var buttonStyle = new GUIStyle(GUI.skin.button);
                    if (_editSpawnPos) GUI.color = Color.green;
                    if (GUILayout.Button("Edit Spawn Position", GUILayout.Height(30f)))
                    {
                        _editSpawnPos = !_editSpawnPos;
                    }
                    GUI.color = Color.white;
                    
                    if (GUILayout.Button("Set Spawn Pos to Object", GUILayout.Height(30f)))
                    {
                        stage.spawnPosition = stage.transform.position;
                    }
                }
                
                stage.spawnPosition = EditorGUILayout.Vector3Field("Spawn Position", stage.spawnPosition);
            }
            
            GUILayout.Space(10f);
            
            // Build Avatar
            using (var areaScope = new GUILayout.VerticalScope("box"))
            {
                AreaTitle("Export Stage");
                
                stage.stageBundleName = EditorGUILayout.TextField("Stage Bundle Name", stage.stageBundleName);
                
                using (new GUILayout.HorizontalScope("box"))
                {
                    var btnstyle = GUI.skin.button;
                    btnstyle.richText = true;
                    if (GUILayout.Button($"<size=14><b><color=white>Build and Save</color></b></size>", GUILayout.Height(40f)))
                    {
                        stage.BuildStage();
                    }
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
        
        protected virtual void OnSceneGUI()
        {
            Reverie_Stage stage = (Reverie_Stage)target;

            if (_editSpawnPos)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition =
                    Handles.PositionHandle(stage.spawnPosition, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(stage, "Change Spawn Position");
                    stage.spawnPosition = newTargetPosition;
                }
            }
        }
    }
}
#endif