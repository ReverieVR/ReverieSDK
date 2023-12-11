using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ReverieSDK
{
    public class Reverie_ARKitFaceTracking : MonoBehaviour
    {
        public enum ARKitBlendshapes
        {
	        None                =   -1,
	        BrowDownLeft        =   0,
	        BrowDownRight       =   1,
	        BrowInnerUp         =   2,
	        BrowOuterUpLeft     =   3,
	        BrowOuterUpRight    =   4,
	        CheekPuff           =   5,
	        CheekSquintLeft     =   6,
	        CheekSquintRight    =   7,
	        EyeBlinkLeft        =   8,
	        EyeBlinkRight       =   9,
	        EyeLookDownLeft     =   10,
	        EyeLookDownRight    =   11,
	        EyeLookInLeft       =   12,
	        EyeLookInRight      =   13,
	        EyeLookOutLeft      =   14,
	        EyeLookOutRight     =   15,
	        EyeLookUpLeft       =   16,
	        EyeLookUpRight      =   17,
	        EyeSquintLeft       =   18,
	        EyeSquintRight      =   19,
	        EyeWideLeft         =   20,
	        EyeWideRight        =   21,
	        JawForward          =   22,
	        JawLeft             =   23,
	        JawOpen             =   24,
	        JawRight            =   25,
	        MouthClose          =   26,
	        MouthDimpleLeft     =   27,
	        MouthDimpleRight    =   28,
	        MouthFrownLeft      =   29,
	        MouthFrownRight     =   30,
	        MouthFunnel         =   31,
	        MouthLeft           =   32,
	        MouthLowerDownLeft  =   33,
	        MouthLowerDownRight =   34,
	        MouthPressLeft      =   35,
	        MouthPressRight     =   36,
	        MouthPucker         =   37,
	        MouthRight          =   38,
	        MouthRollLower      =   39,
	        MouthRollUpper      =   40,
	        MouthShrugLower     =   41,
	        MouthShrugUpper     =   42,
	        MouthSmileLeft      =   43,
	        MouthSmileRight     =   44,
	        MouthStretchLeft    =   45,
	        MouthStretchRight   =   46,
	        MouthUpperUpLeft    =   47,
	        MouthUpperUpRight   =   48,
	        NoseSneerLeft       =   49,
	        NoseSneerRight      =   50,
	        TongueOut           =   51,
	        Max           =   52,
        }

        public int[] blendshapeIndexes;
        public SkinnedMeshRenderer faceMesh;
        public List<string> faceMeshBlendshapeNames = new List<string>();

        private void OnValidate()
        {
            if (blendshapeIndexes == null || blendshapeIndexes.Length < Enum.GetNames(typeof(ARKitBlendshapes)).Length - 1)
            {
                blendshapeIndexes = new int[Enum.GetNames(typeof(ARKitBlendshapes)).Length];
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
            for (int i = 0; i < Enum.GetNames(typeof(ARKitBlendshapes)).Length - 1; i++)
            {
                string nametofind = Enum.GetNames(typeof(ARKitBlendshapes))[i];
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
        
        // Function to get the component in the top-level children
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
