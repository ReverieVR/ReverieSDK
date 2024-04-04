using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ReverieSDK
{
    public class Reverie_BlendshapeDriver : MonoBehaviour
    {
        public string _blendshapeName = "";
        public SkinnedMeshRenderer _skinnedMeshRenderer = null;
        public List<DriverTransformSource> _transformSources = new List<DriverTransformSource>();

        public enum DriverSourceType { None, Transform, Rigidbody, }
        public enum DriverAxis { None, X, Y, Z }
        
        [Serializable]
        public class DriverTransformSource
        {
            public Transform source;
            [Range(0f,1f)] public float weight = 1f;
            public DriverAxis axis = 0;
            [Range(0f,180f)] public float maxAngle = 90f;
            [Range(0f,180f)] public float minAngle = 180f;
        }

        private List<Quaternion> startRotations = new List<Quaternion>();
        private List<Quaternion> prevRotations = new List<Quaternion>();
        
        private void Awake()
        {
            foreach (var transformSource in _transformSources)
            {
                startRotations.Add(transformSource.source.localRotation);
                prevRotations.Add(transformSource.source.localRotation);
            }
        }

        private void Update()
        {
            if (_transformSources.Count <= 0) return;
            
            foreach (var source in _transformSources)
            {
                float rotation = 0;
                
                if (source.axis == DriverAxis.X)
                {
                    Quaternion start = (startRotations[_transformSources.IndexOf(source)]);
                    Quaternion current = source.source.localRotation;
                    rotation = Quaternion.Angle(start, current);
                    
                    rotation = Quaternion.Angle(start, FlattenQuaternion(current, source.source.right));
                    
                    _skinnedMeshRenderer.SetBlendShapeWeight(0, map(Mathf.Abs(rotation), 0, 180, 0, 100) * source.weight);
                }
                else if (source.axis == DriverAxis.Y)
                {
                    //_skinnedMeshRenderer.SetBlendShapeWeight(0, rotation * source.weight);
                }
                else if (source.axis == DriverAxis.Z)
                {
                    //_skinnedMeshRenderer.SetBlendShapeWeight(0, rotation * source.weight);
                }
                
                //prevLocalRotations[_transformSources.IndexOf(source)] = source.source.rotation;
            }
            
            Quaternion FlattenQuaternion(Quaternion original, Vector3 axis)
            {
                // Extract the rotation angle around the specified axis from the original quaternion
                float angle = Quaternion.AngleAxis(Quaternion.Euler(original.eulerAngles).eulerAngles.y, axis).eulerAngles.y;

                // Create a new quaternion with the rotation only around the specified axis
                Quaternion flattenedRotation = Quaternion.AngleAxis(angle, axis);

                return flattenedRotation;
            }
            
            float map(float s, float a1, float a2, float b1, float b2)
            {
                return b1 + (s-a1)*(b2-b1)/(a2-a1);
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(Reverie_BlendshapeDriver))]
    [CanEditMultipleObjects]
    public class Reverie_BlendshapeDriverEditor : Editor
    {
        SerializedProperty _blendshapeName;
        SerializedProperty _skinnedMeshRenderer;
        SerializedProperty _transformSources;
        
        private void OnEnable()
        {
            _blendshapeName = serializedObject.FindProperty("_blendshapeName");
            _skinnedMeshRenderer = serializedObject.FindProperty("_skinnedMeshRenderer");
            _transformSources = serializedObject.FindProperty("_transformSources");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_blendshapeName);
            EditorGUILayout.PropertyField(_skinnedMeshRenderer);
            EditorGUILayout.PropertyField(_transformSources, true);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}