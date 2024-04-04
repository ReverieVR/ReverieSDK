using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReverieSDK
{
    [ExecuteInEditMode]
    public class Reverie_Stage : MonoBehaviour
    {
        public string stageBundleName = "";
        public Vector3 spawnPosition;
        public static event Action<StageInfo> StageLoaded;
        public StageInfo stageInfo = new StageInfo();
        
        [Serializable]
        public class StageInfo
        {
            public Vector3 spawnPosition;
            public Reverie_VideoPlayer[] videoPlayers;
        }
        
        #if UNITY_EDITOR
        private void Awake()
        {
            GetStageInfo();
        }
        #endif

        private void Start()
        {
            if (Application.isPlaying)
            {
                StageLoaded?.Invoke(stageInfo);
            }
        }

        #if UNITY_EDITOR
        private void GetStageInfo()
        {
            if (stageBundleName.Length <= 0) stageBundleName = gameObject.name;
            if (spawnPosition == Vector3.zero) spawnPosition = transform.position;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spawnPosition, 0.1f);
        }
        
        public async void BuildStage()
        {
            if (await BundleExportUtility.CheckForUnsavedScene()) return;
            
            // Create serialized object and apply after updating stage info.
            // This is necessary to ensure the serialized info is saved when bundled.
            SerializedObject serializedObject = new SerializedObject(this);
            
            // Populate stage info
            stageInfo.spawnPosition = spawnPosition;
            stageInfo.videoPlayers = FindObjectsOfType<Reverie_VideoPlayer>();
            
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
            
            if (SceneManager.GetActiveScene().path == "")
            {
                EditorUtility.DisplayDialog("Stage Build Warning",
                    $"Please save your scene before building the stage.",
                    "Ok"
                );
                return;
            }
            
            if (BundleExportUtility.CheckStageForMissingScripts()) { return; }
            BundleExportUtility.StripAudioListeners();
            
            // Create stage asset
            string localPath = SceneManager.GetActiveScene().path;
            string newlocalPath = localPath.Insert(localPath.Length - 6,
                DateTime.UtcNow.ToShortDateString().Replace(" ", "").Replace("/", "").Replace(":", "")
                + DateTime.UtcNow.ToShortTimeString().Replace(" ", "").Replace("/", "").Replace(":", ""));
            
            FileUtil.CopyFileOrDirectory( localPath, newlocalPath);
            AssetDatabase.Refresh();
            
            var asset = AssetImporter.GetAtPath(newlocalPath);
            asset.SetAssetBundleNameAndVariant(stageBundleName + ".rvst", stageBundleName);

            string infoPath  = BundleExportUtility.MakeBundleInfoAsset();
            
            AssetBundleBuild[] ab = new AssetBundleBuild[]
            {
                new AssetBundleBuild()
                {
                    assetBundleName = stageBundleName + ".rvst",
                    assetNames = new[] {newlocalPath}
                }
            };

            
            // Build Asset Bundle
            var bundlePath = Path.GetDirectoryName(Application.persistentDataPath) + "\\Reverie\\StageBundles";
            if (!Directory.Exists(bundlePath)) Directory.CreateDirectory(bundlePath);
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
            File.Delete(infoPath);
            File.Delete(infoPath + ".meta");
            
            Debug.Log("Stage Successfully Exported!");
        }
        
        #endif
    }
}