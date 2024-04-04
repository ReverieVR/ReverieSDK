#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ReverieSDK 
{
    [InitializeOnLoad]
    public static class Reverie_SDK
    {
        private static string[] sdkLayers = new[] {"Default", "TransparentFX", "Ignore Raycast", "", "Water", "UI", "Player", "LocalPlayer",
            "Mirror", "Interaction", "UICollider", "PostProcessing", "PostProcessingCamera", "Camera", "HUD", "HiddenMesh"};
        
        static Reverie_SDK()
        {
            UpdateProjectSettings();
        }
        
        [MenuItem("Reverie SDK/Check Project Settings")]
        static void UpdateProjectSettings()
        {
            bool needsSetup = CheckSettings();
            
            if (needsSetup == false) return;
            
            needsSetup = EditorUtility.DisplayDialog("Setup Reverie SDK",
                $"Your project does not have the correct settings for Reverie SDK. Would you like to set them up now?",
                "Yes",
                "No"
            );
            
            if (needsSetup == false) return;
            
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.companyName = "CASCAS!";
            PlayerSettings.productName = "Reverie";
#pragma warning disable CS0618 // Type or member is obsolete
            PlayerSettings.virtualRealitySupported = true;
#pragma warning restore CS0618 // Type or member is obsolete
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;
            
            // Setup SDK Layers
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            for (int i = 0; i < sdkLayers.Length; i++)
            {
                string originalLayer = layers.GetArrayElementAtIndex(i).stringValue;
                string sdkLayer = sdkLayers[i];
                if (originalLayer != sdkLayer)
                {
                    layers.GetArrayElementAtIndex(i).stringValue = sdkLayer;
                }
            }
            tagManager.ApplyModifiedProperties();
            
            needsSetup = EditorUtility.DisplayDialog("Setup Reverie SDK",
                $"Your project is now setup for the Reverie SDK!",
                "Great!"
            );
        }

        static bool CheckSettings()
        {
            if (PlayerSettings.colorSpace != ColorSpace.Linear) return true;
            if (PlayerSettings.companyName != "CASCAS!") return true;
            if (PlayerSettings.productName != "Reverie") return true;
#pragma warning disable CS0618 // Type or member is obsolete
            if (PlayerSettings.virtualRealitySupported != true) return true;
#pragma warning restore CS0618 // Type or member is obsolete
            if (PlayerSettings.stereoRenderingPath != StereoRenderingPath.Instancing) return true;
            
            // Check SDK Layers
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            for (int i = 0; i < sdkLayers.Length; i++)
            {
                string originalLayer = layers.GetArrayElementAtIndex(i).stringValue;
                string sdkLayer = sdkLayers[i];
                if (originalLayer != sdkLayer) return true;
            }

            return false;
        }

        [MenuItem("Reverie SDK/Open Avatar Folder")]
        static void OpenAvatarFolder()
        {
            if (!Directory.Exists(Path.GetDirectoryName(Application.persistentDataPath) + "\\Reverie\\AvatarBundles"))
                Directory.CreateDirectory(Path.GetDirectoryName(Application.persistentDataPath) + "\\Reverie\\AvatarBundles");
            
            Process.Start("explorer.exe", Path.GetDirectoryName(Application.persistentDataPath) + "\\Reverie\\AvatarBundles");
        }
        
        [MenuItem("Reverie SDK/Open Stage Folder")]
        static void OpenStageFolder()
        {
            if (!Directory.Exists(Path.GetDirectoryName(Application.persistentDataPath) + "\\Reverie\\StageBundles"))
                Directory.CreateDirectory(Path.GetDirectoryName(Application.persistentDataPath) + "\\Reverie\\StageBundles");
            
            Process.Start("explorer.exe", Path.GetDirectoryName(Application.persistentDataPath) + "\\Reverie\\StageBundles");
        }
    }
}
#endif