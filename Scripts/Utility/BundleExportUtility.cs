#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReverieSDK 
{
    public static class BundleExportUtility
    {
        /// <returns>
        /// False if there are no scripts or all scripts have been removed. Returns true if User cancels the build.
        /// </returns>
        public static bool CheckAvatarForMissingScripts(GameObject clone)
        {
            List<GameObject> missingScriptObjs = new List<GameObject>();
            int count = 0;
            foreach (var trans in clone.GetComponentsInChildren<Transform>(true))
            {
                foreach (var comp in trans.GetComponents<Component>())
                {
                    if (comp == null)
                    {
                        missingScriptObjs.Add(trans.gameObject);
                        count += 1;
                    }
                }
            }
            
            if (count == 0) return false;
            
            bool choice = EditorUtility.DisplayDialog("Avatar Build Warning",
                $"You have {count} missing scripts on your avatar and they will be removed. Continue?",
                "Remove and Continue",
                "Cancel Build"
            );
            
            if (choice == false) return true;
            
            foreach (var obj in missingScriptObjs)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
            }

            return false;
        }

        /// <returns>
        /// False if there are no scripts or all scripts have been removed. Returns true if User cancels the build.
        /// </returns>
        public static bool CheckStageForMissingScripts()
        {
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>() ;
            List<GameObject> missingScriptObjs = new List<GameObject>();
            int count = 0;
            foreach (var obj in allObjects)
            {
                foreach (var comp in obj.GetComponentsInChildren<Component>(true))
                {
                    if (comp == null)
                    {
                        missingScriptObjs.Add(obj);
                        count += 1;
                    }
                }
            }
            
            if (count == 0) return false;
            
            bool choice = EditorUtility.DisplayDialog("Stage Build Warning",
                $"You have {count} missing scripts in your scene and they will be removed. Continue?",
                "Remove and Continue",
                "Cancel Build"
            );
            
            if (choice == false) return true;
            
            foreach (var obj in missingScriptObjs)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
            }

            return false;
        }
        
        /// <returns>
        /// True User does not want to save scene. False user wants to save scene.
        /// </returns>
        public static async Task<bool> CheckForUnsavedScene()
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene.isDirty)
            {
                bool choice = EditorUtility.DisplayDialog("Avatar Build Warning",
                    $"Your scene is current unsaved. Save and continue?",
                    "Save",
                    "Cancel"
                );
            
                if (choice == false) return true;
                
                await Task.Run(() =>
                {
                    EditorApplication.delayCall += () =>
                    {
                        EditorSceneManager.SaveScene(activeScene);
                    };
                });
            }
            return false;
        }
        
        public static void StripAudioListeners(GameObject avatar = null)
        {
            if (avatar == null)
            {
                GameObject[] allObjects = Object.FindObjectsOfType<GameObject>() ;
                foreach (var obj in allObjects)
                {
                    foreach (var comp in obj.GetComponentsInChildren<AudioListener>(true))
                    {
                        Object.DestroyImmediate(comp);
                    }
                }
            }
            else
            {
                foreach (var comp in avatar.GetComponentsInChildren<AudioListener>(true))
                {
                    Object.DestroyImmediate(comp);
                }
            }
        }
        
        public static async Task<string> GetShaderVariants(string bundleName)
        {
            if (bundleName == null) Debug.LogError("Avatar Bundle Name is null. Cannot save Shader Variants.");
            
            //Clear Variant Collection
            typeof(ShaderUtil).GetMethod("ClearCurrentShaderVariantCollection", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[0]);

            await Task.Delay(2000);
          
            //Save Variant Collection
            if (!Directory.Exists("Assets/ShaderVariants")) Directory.CreateDirectory("Assets/ShaderVariants");
            
            typeof(ShaderUtil).GetMethod("SaveCurrentShaderVariantCollection", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { "Assets/ShaderVariants/" + bundleName + ".shadervariants" });

            return "Assets/ShaderVariants/" + bundleName + ".shadervariants";
        }

        public static string MakeBundleInfoAsset()
        {
            // Get version number from Version.txt
            string versionNumber = File.ReadAllText("Assets/ReverieSDK/Version.txt").Split('\n')[0].Split(':')[1];
            string currentDate = DateTime.UtcNow.ToShortDateString().Replace(" ", "").Replace("/", "").Replace(":", "");
            string avatarInfo = $"SDKVersion:{versionNumber}\nDateCreated:{currentDate}";

            string path = "Assets/AvatarInfo.txt";
            File.WriteAllText(path, avatarInfo);

            AssetDatabase.Refresh();
            
            return path;
        }
    }
}

#endif