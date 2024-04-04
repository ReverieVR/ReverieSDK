using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ReverieSDK
{
    public class Reverie_MetaFaceTracking : MonoBehaviour
    {
        public enum FBExpression
        {
            None  =   -1,
            BrowLowererL = 0,
            BrowLowererR = 1,
            CheekPuffL = 2,
            CheekPuffR = 3,
            CheekRaiserL = 4,
            CheekRaiserR = 5,
            CheekSuckL = 6,
            CheekSuckR = 7,
            ChinRaiserB = 8,
            ChinRaiserT = 9,
            DimplerL = 10,
            DimplerR = 11,
            EyesClosedL = 12,
            EyesClosedR = 13,
            EyesLookDownL = 14,
            EyesLookDownR = 15,
            EyesLookLeftL = 16,
            EyesLookLeftR = 17,
            EyesLookRightL = 18,
            EyesLookRightR = 19,
            EyesLookUpL = 20,
            EyesLookUpR = 21,
            InnerBrowRaiserL = 22,
            InnerBrowRaiserR = 23,
            JawDrop = 24,
            JawSidewaysLeft = 25,
            JawSidewaysRight = 26,
            JawThrust = 27,
            LidTightenerL = 28,
            LidTightenerR = 29,
            LipCornerDepressorL = 30,
            LipCornerDepressorR = 31,
            LipCornerPullerL = 32,
            LipCornerPullerR = 33,
            LipFunnelerLB = 34,
            LipFunnelerLT = 35,
            LipFunnelerRB = 36,
            LipFunnelerRT = 37,
            LipPressorL = 38,
            LipPressorR = 39,
            LipPuckerL = 40,
            LipPuckerR = 41,
            LipStretcherL = 42,
            LipStretcherR = 43,
            LipSuckLB = 44,
            LipSuckLT = 45,
            LipSuckRB = 46,
            LipSuckRT = 47,
            LipTightenerL = 48,
            LipTightenerR = 49,
            LipsToward = 50,
            LowerLipDepressorL = 51,
            LowerLipDepressorR = 52,
            MouthLeft = 53,
            MouthRight = 54,
            NoseWrinklerL = 55,
            NoseWrinklerR = 56,
            OuterBrowRaiserL = 57,
            OuterBrowRaiserR = 58,
            UpperLidRaiserL = 59,
            UpperLidRaiserR = 60,
            UpperLipRaiserL = 61,
            UpperLipRaiserR = 62,
            Max = 63
        }

        public int[] blendshapeIndexes;
        public SkinnedMeshRenderer faceMesh;
        public List<string> faceMeshBlendshapeNames = new List<string>();

        private void OnValidate()
        {
            if (blendshapeIndexes == null || blendshapeIndexes.Length < Enum.GetNames(typeof(FBExpression)).Length)
            {
                blendshapeIndexes = new int[Enum.GetNames(typeof(FBExpression)).Length];
                for (var i = 0; i < blendshapeIndexes.Length; i++)
                {
                    blendshapeIndexes[i] = 0;
                }
            }

            if (faceMesh == null)
            {
                faceMesh = GetTopLevelChildComponent<SkinnedMeshRenderer>();
            }
            
            if (faceMeshBlendshapeNames.Count < faceMesh.sharedMesh.blendShapeCount || faceMeshBlendshapeNames == null)
            {
                faceMeshBlendshapeNames = new List<string>();
                faceMeshBlendshapeNames.Add("-none-");
                for (int i = 0; i < faceMesh.sharedMesh.blendShapeCount - 1; i++)
                {
                    faceMeshBlendshapeNames.Add(faceMesh.sharedMesh.GetBlendShapeName(i));
                }
                
                //MatchNamesToIndexes();
            }
        }
        
        public void MatchNamesToIndexes()
        {
            for (int i = 0; i < Enum.GetNames(typeof(FBExpression)).Length - 1; i++)
            {
                string nametofind = Enum.GetNames(typeof(FBExpression))[i];
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
        
        public static (string, FBExpression)[] ARKitConversion =
            {
            ("EyeBlinkLeft", FBExpression.EyesClosedL),
            ("EyeLookDownLeft", FBExpression.EyesLookDownL),
            ("EyeLookInLeft", FBExpression.EyesLookRightL),
            ("EyeLookOutLeft",FBExpression.EyesLookLeftL),
            ("EyeLookUpLeft",FBExpression.EyesLookUpL),
            ("EyeSquintLeft",FBExpression.LidTightenerL),
            ("EyeWideLeft",FBExpression.UpperLidRaiserL),
            ("EyeBlinkRight",FBExpression.EyesClosedR),
            ("EyeLookDownRight",FBExpression.EyesLookDownR),
            ("EyeLookInRight",FBExpression.EyesLookLeftR),
            ("EyeLookOutRight",FBExpression.EyesLookRightR),
            ("EyeLookUpRight",FBExpression.EyesLookUpR),
            ("EyeSquintRight",FBExpression.LidTightenerR),
            ("EyeWideRight",FBExpression.UpperLidRaiserR),
            ("JawForward",FBExpression.JawThrust),
            ("JawLeft",FBExpression.JawSidewaysLeft),
            ("JawRight",FBExpression.JawSidewaysRight),
            ("JawOpen",FBExpression.JawDrop),
            ("MouthClose",FBExpression.LipsToward),
            ("MouthFunnel",FBExpression.LipFunnelerLB),
            ("MouthPucker",FBExpression.LipPuckerL),
            ("MouthLeft",FBExpression.MouthLeft),
            ("MouthRight",FBExpression.MouthRight),
            ("MouthSmileLeft",FBExpression.LipCornerPullerL),
            ("MouthSmileRight",FBExpression.LipCornerPullerR),
            ("MouthFrownLeft",FBExpression.LipCornerDepressorL),
            ("MouthFrownRight",FBExpression.LipCornerDepressorR),
            ("MouthDimpleLeft",FBExpression.DimplerL),
            ("MouthDimpleRight",FBExpression.DimplerR),
            ("MouthStretchLeft",FBExpression.LipStretcherL),
            ("MouthStretchRight",FBExpression.LipStretcherR),
            ("MouthRollLower",FBExpression.LipSuckLB),
            ("MouthRollUpper",FBExpression.LipSuckLT),
            ("MouthShrugLower",FBExpression.ChinRaiserB),
            ("MouthShrugUpper",FBExpression.ChinRaiserT),
            ("MouthPressLeft",FBExpression.LipPressorL),
            ("MouthPressRight",FBExpression.LipPressorR),
            ("MouthLowerDownLeft",FBExpression.LowerLipDepressorL),
            ("MouthLowerDownRight",FBExpression.LowerLipDepressorR),
            ("MouthUpperUpLeft",FBExpression.UpperLipRaiserL),
            ("MouthUpperUpRight",FBExpression.UpperLipRaiserR),
            ("BrowDownLeft",FBExpression.BrowLowererL),
            ("BrowDownRight",FBExpression.BrowLowererR),
            ("BrowInnerUp",FBExpression.InnerBrowRaiserL),
            ("BrowOuterUpLeft",FBExpression.OuterBrowRaiserL),
            ("BrowOuterUpRight",FBExpression.OuterBrowRaiserR),
            ("CheekPuff",FBExpression.CheekPuffL),
            ("CheekSquintLeft",FBExpression.CheekRaiserL),
            ("CheekSquintRight",FBExpression.CheekRaiserR),
            ("NoseSneerLeft",FBExpression.NoseWrinklerL),
            ("NoseSneerRight",FBExpression.NoseWrinklerR)
        };

        public void GetBLendshapeIndexesFromARKit()
        {
            for (int i = 0; i < ARKitConversion.Length - 1; i++)
            {
                string nametofind = ARKitConversion[i].Item1;
                int blendshapeIndex = 0;
                
                for (int j = 0; j < faceMesh.sharedMesh.blendShapeCount; j++)
                {
                    string blendshapeName = faceMesh.sharedMesh.GetBlendShapeName(j).ToLower().Replace("_", "");
                    if (blendshapeName.Contains(nametofind.ToLower().Replace("_", "")))
                    {
                        blendshapeIndex = j;
                        Debug.Log("Found " + nametofind);   
                        Debug.Log("FBExIndex: " + (int)ARKitConversion[i].Item2);   
                        blendshapeIndexes[(int)ARKitConversion[i].Item2] = blendshapeIndex+1;
                        break;
                    }
                }
            }
        }
    }
}
