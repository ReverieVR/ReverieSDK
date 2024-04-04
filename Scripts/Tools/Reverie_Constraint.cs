using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReverieSDK
{
    public class Reverie_Constraint : MonoBehaviour
    {
        [SerializeField] public List<ConstraintSource> _constraintSources = new List<ConstraintSource>();
        
        public enum ConstraintAxis { None, X, Y, Z }
        
        [Serializable]
        public class ConstraintSource
        {
            public Transform sourceTransform;
            [Range(0,1)] public float weight = 1;
            public ConstraintAxis axis = 0;
            public bool useLocalSpace = false;
            [HideInInspector] public Quaternion sourceOffset;
            [HideInInspector] public Quaternion previousRotation;
            [HideInInspector] public Quaternion startLocalRotation;
        }
        
        public Quaternion FlattenQuaternion(Quaternion original, Vector3 axis)
        {
            // Extract the rotation angle around the specified axis from the original quaternion
            float angle = Quaternion.AngleAxis(Quaternion.Euler(original.eulerAngles).eulerAngles.x, axis).eulerAngles.x;
            
            // Create a new quaternion with the rotation only around the specified axis
            Quaternion flattenedRotation = Quaternion.AngleAxis(angle, axis);

            return flattenedRotation;
        }
    }
}