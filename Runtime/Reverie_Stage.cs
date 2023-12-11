using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Cascadian.SDK
{
    [ExecuteInEditMode]
    public class Reverie_Stage : MonoBehaviour
    {
        public string stageBundleName;
        public Vector3 spawnPosition;
        public static event Action<Vector3> SetPlayerSpawn;
        
        private void Awake()
        {
            GetStageInfo();
            
            if (Application.isPlaying)
            {
                SetPlayerSpawn?.Invoke(spawnPosition);
            }
        }
        
        private void GetStageInfo()
        {
            if (stageBundleName.Length <= 0) stageBundleName = gameObject.name;
            if (spawnPosition == Vector3.zero) spawnPosition = transform.position;
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spawnPosition, 0.1f);
        }
        
        public void BuildStage()
        {
            bool removeScripts = CheckForMissingScripts();
            if (removeScripts == false) return;
            
            string localPath = SceneManager.GetActiveScene().path;
            string newlocalPath = localPath.Insert(localPath.Length - 6,
                DateTime.UtcNow.ToShortDateString().Replace(" ", "").Replace("/", "").Replace(":", "")
                + DateTime.UtcNow.ToShortTimeString().Replace(" ", "").Replace("/", "").Replace(":", ""));
            
            FileUtil.CopyFileOrDirectory( localPath, newlocalPath);
            AssetDatabase.Refresh();
            
            var asset = AssetImporter.GetAtPath(newlocalPath);
            asset.SetAssetBundleNameAndVariant(stageBundleName + ".rvst", stageBundleName);

            AssetBundleBuild[] ab = new AssetBundleBuild[]
            {
                new AssetBundleBuild()
                {
                    assetBundleName = stageBundleName + ".rvst",
                    assetNames = new[] {newlocalPath}
                }
            };

            var bundlePath = Path.GetDirectoryName(Application.persistentDataPath) + "\\Reverie\\StageBundles";
            
            if (!Directory.Exists(bundlePath))
                Directory.CreateDirectory(bundlePath);
            
            BuildPipeline.BuildAssetBundles (
                Path.GetDirectoryName(Application.persistentDataPath) + "\\Reverie\\StageBundles",
                ab, 
                BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget
            );
            
            File.Delete(bundlePath + "\\" + stageBundleName + ".rvst.manifest");
            File.Delete(bundlePath + "\\StageBundles");
            File.Delete(bundlePath + "\\StageBundles.manifest");
            AssetDatabase.DeleteAsset(newlocalPath);
            
            Debug.Log("Stage Successfully Exported!");
        }

        public bool CheckForMissingScripts()
        {
            var objs = Resources.FindObjectsOfTypeAll<GameObject>();
            int count = objs.Sum(GameObjectUtility.RemoveMonoBehavioursWithMissingScript);
            
            if (count == 0) return true;
            bool choice = EditorUtility.DisplayDialog("Stage Build Warning",
                "You have missing scripts in your scene and they will be removed. Continue?",
                "Remove and Continue",
                "Cancel Build"
            );
            return choice;
        }
        
        #endif
    }
}