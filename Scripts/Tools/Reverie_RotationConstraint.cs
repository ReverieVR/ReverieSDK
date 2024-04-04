using UnityEditor;
using UnityEngine;

namespace ReverieSDK
{
    public class Reverie_RotationConstraint : Reverie_Constraint
    {
        private void Awake()
        {
            if (_constraintSources.Count <= 0) return;

            foreach (var source in _constraintSources)
            {
                source.sourceOffset = Quaternion.Inverse(source.sourceTransform.rotation) * transform.rotation;
                source.previousRotation = source.sourceTransform.localRotation;
                source.startLocalRotation = transform.localRotation;
            }
        }

        private void LateUpdate()
        {
            if (_constraintSources.Count <= 0) return;
            
            Quaternion[] rotations = new Quaternion[_constraintSources.Count];
            
            foreach (ConstraintSource source in _constraintSources)
            {
                if (source.useLocalSpace)
                {
                    var localRotation = source.sourceTransform.localRotation;
                    Quaternion deltaRotation = Quaternion.Inverse(source.previousRotation) * localRotation;
                    
                    if (source.axis == ConstraintAxis.X)
                    {
                        deltaRotation = Quaternion.Euler(deltaRotation.eulerAngles.x, 0f, 0f);
                    }
                    else if (source.axis == ConstraintAxis.Y)
                    {
                        deltaRotation = Quaternion.Euler(0f, deltaRotation.eulerAngles.y, 0f);
                    }
                    else if (source.axis == ConstraintAxis.Z)
                    {
                        deltaRotation = Quaternion.Euler(0f, 0f, deltaRotation.eulerAngles.z);
                    }

                    deltaRotation = transform.localRotation * deltaRotation;
                    
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, deltaRotation, source.weight);
                    
                    //Quaternion worldDelta = transform.rotation * deltaRotation;
                    //transform.rotation = Quaternion.Lerp(transform.rotation, worldDelta, source.weight);
                    
                    source.previousRotation = localRotation;
                }
                else
                {
                    Quaternion rotation = source.sourceTransform.rotation * source.sourceOffset;
                    
                    if (source.axis == ConstraintAxis.X)
                    {
                        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);
                        float rotationX = Mathf.Rad2Deg * Mathf.Atan2(rotationMatrix.m21, rotationMatrix.m22);
                        rotation = Quaternion.Euler(rotationX, 0f, 0f);
                    }
                    else if (source.axis == ConstraintAxis.Y)
                    {
                        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);
                        float rotationY = Mathf.Rad2Deg * Mathf.Atan2(rotationMatrix.m13, rotationMatrix.m33);
                        rotation = Quaternion.Euler(0f, rotationY, 0f);
                    }
                    else if (source.axis == ConstraintAxis.Z)
                    {
                        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);
                        float rotationZ = Mathf.Rad2Deg * Mathf.Atan2(rotationMatrix.m12, rotationMatrix.m11);
                        rotation = Quaternion.Euler(0f, 0f, rotationZ);
                    }
                    
                    transform.rotation = Quaternion.Lerp(transform.rotation,rotation, source.weight);
                    rotations[_constraintSources.IndexOf(source)] = Quaternion.Lerp(transform.rotation,rotation, source.weight);
                }
            }
            
            
        }
    }

    #if UNITY_EDITOR
    
    [CustomEditor(typeof(Reverie_RotationConstraint))]
    [CanEditMultipleObjects]
    public class Reverie_RotationConstraintEditor : Editor
    {
        SerializedProperty _constraintSources;
        
        private void OnEnable()
        {
            _constraintSources = serializedObject.FindProperty("_constraintSources");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_constraintSources, true);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}