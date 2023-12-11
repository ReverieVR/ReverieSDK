using System;
using UnityEditor;
using UnityEngine;

namespace ReverieSDK
{
    [CustomEditor(typeof(Reverie_ARKitFaceTracking))]
    public class Subworks_ARKitFaceTrackingEditor : Editor
    {
        private bool _foldoutToggle;
        
        public override void OnInspectorGUI()
        {
            Reverie_ARKitFaceTracking arkitFaceTracking = (Reverie_ARKitFaceTracking)target;
            
            EditorGUILayout.ObjectField("Face Mesh: ", arkitFaceTracking.faceMesh, typeof(SkinnedMeshRenderer), true);
            
            _foldoutToggle = EditorGUILayout.Foldout(_foldoutToggle,"Blendshapes: ");

            if (_foldoutToggle)
            {
                for (int i = 0; i < Enum.GetNames(typeof(Reverie_ARKitFaceTracking.ARKitBlendshapes)).Length - 1; i++)
                {
                    arkitFaceTracking.blendshapeIndexes[i] = EditorGUILayout.Popup(
                        Enum.GetNames(typeof(Reverie_ARKitFaceTracking.ARKitBlendshapes))[i],
                        arkitFaceTracking.blendshapeIndexes[i],
                        arkitFaceTracking.faceMeshBlendshapeNames.ToArray());
                }
            }
        }
    }
}