using System;
using UnityEditor;
using UnityEngine;

namespace ReverieSDK
{
    [CustomEditor(typeof(Reverie_SRanipalFaceTracking))]
    public class Reverie_SRanipalFaceTrackingEditor : Editor
    {
        private bool _foldoutToggle;
        
        public override void OnInspectorGUI()
        {
            Reverie_SRanipalFaceTracking sranipalFaceTracking = (Reverie_SRanipalFaceTracking)target;
            
            EditorGUILayout.ObjectField("Face Mesh: ", sranipalFaceTracking.faceMesh, typeof(SkinnedMeshRenderer), true);
            
            _foldoutToggle = EditorGUILayout.Foldout(_foldoutToggle,"Blendshapes: ");

            if (_foldoutToggle)
            {
                for (int i = 0; i < Enum.GetNames(typeof(Reverie_SRanipalFaceTracking.SRanipalExpression)).Length - 1; i++)
                {
                    sranipalFaceTracking.blendshapeIndexes[i] = EditorGUILayout.Popup(
                        Enum.GetNames(typeof(Reverie_SRanipalFaceTracking.SRanipalExpression))[i],
                        sranipalFaceTracking.blendshapeIndexes[i],
                        sranipalFaceTracking.faceMeshBlendshapeNames.ToArray());
                }
            }
        }
    }
}