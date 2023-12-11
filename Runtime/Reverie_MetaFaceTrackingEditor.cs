using System;
using UnityEditor;
using UnityEngine;

namespace Cascadian.SDK
{
    [CustomEditor(typeof(Reverie_MetaFaceTracking))]
    public class Subworks_MetaFaceTrackingEditor : Editor
    {
        private bool _foldoutToggle;
        
        public override void OnInspectorGUI()
        {
            Reverie_MetaFaceTracking metaFaceTracking = (Reverie_MetaFaceTracking)target;
            
            EditorGUILayout.ObjectField("Face Mesh: ", metaFaceTracking.faceMesh, typeof(SkinnedMeshRenderer), true);
            
            _foldoutToggle = EditorGUILayout.Foldout(_foldoutToggle,"Blendshapes: ");

            if (_foldoutToggle)
            {
                for (int i = 0; i < Enum.GetNames(typeof(Reverie_MetaFaceTracking.FBExpression)).Length - 1; i++)
                {
                    metaFaceTracking.blendshapeIndexes[i] = EditorGUILayout.Popup(
                        Enum.GetNames(typeof(Reverie_MetaFaceTracking.FBExpression))[i],
                        metaFaceTracking.blendshapeIndexes[i],
                        metaFaceTracking.faceMeshBlendshapeNames.ToArray());
                }
            }
        }
    }
}