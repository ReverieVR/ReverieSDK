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
using System.Text.Json;
using Debug = UnityEngine.Debug;

namespace ReverieSDK 
{
    [InitializeOnLoad]
    public static class Reverie_SDK
    {
        static Reverie_SDK()
        {
            CheckForUpdates();
            CheckProjectSettings();
        }

        [MenuItem("Reverie SDK/Check for Updates")]
        static async void CheckForUpdates()
        {
            string currentVersion = File.ReadAllText(Application.dataPath + "/ReverieSDK/version.json");
            currentVersion = currentVersion.Split(':')[1].Split("\"")[1];

            WebClient wc = new WebClient();
            string newVersion = wc.DownloadString("https://raw.githubusercontent.com/ReverieVR/ReverieSDK/main/version.json");
            newVersion = newVersion.Split(':')[1].Split("\"")[1];
            
            Debug.Log("Current Reverie SDK Version: " + currentVersion);
            Debug.Log("Latest Reverie SDK Version: " + newVersion);

            bool needsUpdate = Convert.ToInt16(currentVersion.Replace(".", "")) < Convert.ToInt16(newVersion.Replace(".", ""));
            
            if (needsUpdate)
            {
                bool update = EditorUtility.DisplayDialog("Reverie SDK Updater",
                    $"Current SDK Version: {currentVersion}\nLatest SDK Version: {newVersion}\n\n" +
                    $"An update is available! Would you like to update now?",
                    "Yes",
                    "No"
                );
                
                if (update == false) return;
                
                Debug.Log("Updating to Latest SDK Version " + newVersion + "...");
                
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.Add(
                    new ProductInfoHeaderValue("ReverieSDK", "1"));
                var repo = "ReverieVR/ReverieSDK";
                var contentsUrl = $"https://api.github.com/repos/{repo}/contents";
                var contentsJson = await httpClient.GetStringAsync(contentsUrl);
            
                JsonDocument contents = JsonDocument.Parse(contentsJson);
                JsonElement root = contents.RootElement;

                // Loop through each element in the array
                foreach (JsonElement element in root.EnumerateArray())
                {
                    // Access the "download_url" property from each object
                    string downloadUrl = element.GetProperty("download_url").GetString();
                    if (downloadUrl == null) continue;
                    if (downloadUrl.Contains(".unitypackage") && !downloadUrl.Contains(".meta"))
                    {
                        string path = Application.dataPath + "/ReverieSDK/";
                        path += downloadUrl.Split('/')[downloadUrl.Split('/').Length - 1];
                        wc.DownloadFile (downloadUrl, path);
                        AssetDatabase.ImportPackage(path, false);
                    }
                }
            }
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