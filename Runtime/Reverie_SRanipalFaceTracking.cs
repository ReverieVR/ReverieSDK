using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ReverieSDK
{
    public class Reverie_SRanipalFaceTracking : MonoBehaviour
    {
        public enum SRanipalExpression
        {
            None = -1,
            Jaw_Right = 0,
            Jaw_Left = 1,
            Jaw_Forward = 2,
            Jaw_Open = 3,
            Mouth_Ape_Shape = 4,
            Mouth_Upper_Right = 5,
            Mouth_Upper_Left = 6,
            Mouth_Lower_Right = 7,
            Mouth_Lower_Left = 8,
            Mouth_Upper_Overturn = 9,
            Mouth_Lower_Overturn = 10,
            Mouth_Pout = 11,
            Mouth_Smile_Right = 12,
            Mouth_Smile_Left = 13,
            Mouth_Sad_Right = 14,
            Mouth_Sad_Left = 15,
            Cheek_Puff_Right = 16,
            Cheek_Puff_Left = 17,
            Cheek_Suck = 18,
            Mouth_Upper_UpRight = 19,
            Mouth_Upper_UpLeft = 20,
            Mouth_Lower_DownRight = 21,
            Mouth_Lower_DownLeft = 22,
            Mouth_Upper_Inside = 23,
            Mouth_Lower_Inside = 24,
            Mouth_Lower_Overlay = 25,
            Tongue_LongStep1 = 26,
            Tongue_LongStep2 = 32,
            Tongue_Down = 30,
            Tongue_Up = 29,
            Tongue_Right = 28,
            Tongue_Left = 27,
            Tongue_Roll = 31,
            Tongue_UpLeft_Morph = 34,
            Tongue_UpRight_Morph = 33,
            Tongue_DownLeft_Morph = 36,
            Tongue_DownRight_Morph = 35,
            Max = 37,
        }

        public int[] blendshapeIndexes;
        public SkinnedMeshRenderer faceMesh;
        public List<string> faceMeshBlendshapeNames = new List<string>();

        private void OnValidate()
        {
            if (blendshapeIndexes == null || blendshapeIndexes.Length < Enum.GetNames(typeof(SRanipalExpression)).Length - 1)
            {
                blendshapeIndexes = new int[Enum.GetNames(typeof(SRanipalExpression)).Length];
            }

            if (faceMesh == null)
            {
                faceMesh = GetTopLevelChildComponent<SkinnedMeshRenderer>();
            }
            
            if (faceMeshBlendshapeNames.Count < faceMesh.sharedMesh.blendShapeCount || faceMeshBlendshapeNames == null)
            {
                faceMeshBlendshapeNames = new List<string>();
                for (int i = 0; i < faceMesh.sharedMesh.blendShapeCount; i++)
                {
                    faceMeshBlendshapeNames.Add(faceMesh.sharedMesh.GetBlendShapeName(i));
                }
                
                MatchNamesToIndexes();
            }
        }
        
        public void MatchNamesToIndexes()
        {
            for (int i = 0; i < Enum.GetNames(typeof(SRanipalExpression)).Length - 1; i++)
            {
                string nametofind = Enum.GetNames(typeof(SRanipalExpression))[i];
                int blendshapeIndex = 0;
                for (int j = 0; j < faceMesh.sharedMesh.blendShapeCount; j++)
                {
                    if (faceMesh.sharedMesh.GetBlendShapeName(j).ToLower().Contains(nametofind.ToLower()))
                    {
                        blendshapeIndex = j;
                        break;
                    }
                }
                
                blendshapeIndexes[i] = blendshapeIndex;
            }
        }
        
        T GetTopLevelChildComponent<T>() where T : Component
        {
            T[] components = GetComponentsInChildren<T>(true);

            foreach (T component in components)
            {
                // Check if the component is a direct child (not nested in other children)
                if (component.transform.parent == transform)
                {
                    return component;
                }
            }

            return null; // Component not found
        }
    }
}
