#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ReverieSDK
{
    [InitializeOnLoad]
    public static class Reverie_SDK
    {
        static Reverie_SDK()
        {
            CheckProjectSettings();
        }

        [MenuItem("Reverie SDK/Check Project Settings")]
        static void CheckProjectSettings()
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