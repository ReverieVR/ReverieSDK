using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ReverieSDK
{
    [ExecuteInEditMode]
    public class Reverie_Avatar : MonoBehaviour
    {
        public enum _visemes
        {
            sil,
            PP,
            FF,
            TH,
            DD,
            kk,
            CH,
            SS,
            nn,
            RR,
            aa,
            E,
            ih,
            oh,
            ou
        }
        
        public enum EyeConstraintDirection
        {
            None,
            Left,
            Right,
            Up,
            Down
        }   
        
        public RuntimeAnimatorController avatarController;
        public int[] visemeList = new int[0];
        public Vector3 eyePositon;
        public Vector3 mouthPositon;
        public SkinnedMeshRenderer faceMesh;
        public Transform leftEye;
        public Transform rightEye;
        public Vector3 leftEyeForward;
        public Vector3 rightEyeForward;
        public float angleLimitUp = 10f;
        public float angleLimitDown = 10f;
        public float angleLimitLeft = 20f;
        public float angleLimitRight = 20f;
        public int leftEyeBlinkIndex = -1;
        public int rightEyeBlinkIndex = -1; 
        public string avatarBundleName = null;
        public List<string> faceMeshBlendshapeNames = new List<string>();
        public Animator avatarAnimator;
        public bool usingEyes = true;
        #if UNITY_EDITOR
        
        private void Awake()
        {
            GetDefaultValues();
        }
        
        public void GetDefaultValues()
        {
            if (avatarBundleName == null) avatarBundleName = gameObject.name;
            avatarAnimator = GetComponent<Animator>();
            if (avatarAnimator == null) return;
            leftEye = avatarAnimator.GetBoneTransform(HumanBodyBones.LeftEye);
            rightEye = avatarAnimator.GetBoneTransform(HumanBodyBones.RightEye);
            if (leftEye == null || rightEye == null)
            {
                usingEyes = false;
                eyePositon = new Vector3(0, 1.75f, 0.1f);
            }
            else
            {
                usingEyes = true;
                leftEyeForward = leftEye.localEulerAngles;
                rightEyeForward = rightEye.localEulerAngles;
                if (eyePositon == Vector3.zero) eyePositon = ((leftEye.position + rightEye.position) / 2.0f) - transform.position;
            }
            if (mouthPositon == Vector3.zero) mouthPositon = avatarAnimator.GetBoneTransform(HumanBodyBones.Head).position - transform.position;

            GetFaceInfo();
        }

        public void GetFaceInfo()
        {
            if (faceMesh == null)
            {
                var topLevelChildren = GetTopLevelChildren(transform);
                foreach (var child in topLevelChildren)
                {
                    var mesh = child.GetComponent<SkinnedMeshRenderer>();
                    if (mesh == null) continue;
                    if (new[] { "face", "head", "body" }.Any(c => mesh.name.ToLower().Contains(c)))
                    {
                        faceMesh = mesh;
                        break;
                    }
                }
                if (faceMesh == null) faceMesh = GetComponentInChildren<SkinnedMeshRenderer>();
            }
            
            if (faceMesh != null)
            {
                faceMeshBlendshapeNames.Clear();
                bool leftBlinkFound = false;
                bool rightBlinkFound = false;
                
                // Get all blenshape names
                for (int i = 0; i < faceMesh.sharedMesh.blendShapeCount; i++)
                {
                    faceMeshBlendshapeNames.Add(faceMesh.sharedMesh.GetBlendShapeName(i));
                }
                
                if (leftEyeBlinkIndex >= 0 || rightEyeBlinkIndex >= 0) return;
                
                //Find Left Eye Blink
                for (int i = 0; i < faceMesh.sharedMesh.blendShapeCount; i++)
                {
                    var name = faceMesh.sharedMesh.GetBlendShapeName(i).ToLower();
                    
                    if (name.Contains("blink") && name.Contains("left"))
                    {
                        leftBlinkFound = true;
                        leftEyeBlinkIndex = i;
                        break;
                    }
                }
                
                //Find Right Eye Blink
                for (int i = 0; i < faceMesh.sharedMesh.blendShapeCount; i++)
                {
                    var name = faceMesh.sharedMesh.GetBlendShapeName(i).ToLower();
                    if (name.Contains("blink") && name.Contains("right"))
                    {
                        rightBlinkFound = true;
                        rightEyeBlinkIndex = i;
                        break;
                    }
                }
                
                if (!leftBlinkFound || !rightBlinkFound)
                {
                    for (int i = 0; i < faceMesh.sharedMesh.blendShapeCount; i++)
                    {
                        var name = faceMesh.sharedMesh.GetBlendShapeName(i);
                        if (name.ToLower().Contains("blink"))
                        {
                            leftEyeBlinkIndex = i;
                            rightEyeBlinkIndex = i;
                        }
                    }
                }
            }
            
            if (visemeList.Length < 15) visemeList = new int[15];
            AutoFillVisemes();
        }
        
        private void OnDrawGizmosSelected()
        {
            // Eye Pos
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(eyePositon + transform.position, 0.01f);
            
            // Mouth Pos
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(mouthPositon + transform.position, 0.01f);
        }

        public void AutoFillVisemes()
        {
            visemeList = new int[Enum.GetNames(typeof(_visemes)).Length];
            
            for (int i = 0; i < Enum.GetNames(typeof(_visemes)).Length; i++)
            {
                string nametofind = Enum.GetNames(typeof(_visemes))[i];
                int blendshapeIndex = 0;
                bool found = false;
                
                for (int j = 0; j < faceMesh.sharedMesh.blendShapeCount; j++)
                {
                    string blendshapeName = faceMesh.sharedMesh.GetBlendShapeName(j).ToLower();
                    if (blendshapeName.Contains(nametofind.ToLower()) && blendshapeName.Contains("vrc"))
                    {
                        found = true;
                        blendshapeIndex = j;
                        break;
                    }
                }

                if (!found)
                {
                    for (int j = 0; j < faceMesh.sharedMesh.blendShapeCount; j++)
                    {
                        string currentShapeName = faceMesh.sharedMesh.GetBlendShapeName(j).ToLower();
                        if (currentShapeName.Contains(nametofind.ToLower()))
                        {
                            found = true;
                            blendshapeIndex = j;
                            break;
                        }
                    }
                }
                
                visemeList[i] = blendshapeIndex;
            }
        }
        
        public void BuildAvatar()
        {
            GameObject clone = Instantiate(gameObject);
            
            bool removeScripts = CheckForMissingScripts(clone);
            if (removeScripts == false) return;

            ReplaceDynamicBoneWithData(clone);
            
            string localPath = "Assets/" + avatarBundleName + ".prefab";

            // Create the new Prefab and log whether Prefab was saved successfully.
            bool prefabSuccess = PrefabUtility.SaveAsPrefabAsset(clone, localPath, out prefabSuccess);
            if (prefabSuccess == false) Debug.LogError("Prefab failed to save" + prefabSuccess);

            
            var asset = AssetImporter.GetAtPath(localPath);
            asset.SetAssetBundleNameAndVariant(avatarBundleName + ".rvav", "LocalAvatar");

            AssetBundleBuild[] ab = new AssetBundleBuild[]
            {
                new AssetBundleBuild()
                {
                    assetBundleName = avatarBundleName + ".rvav",
                    assetNames = new[] {localPath}
                }
            };

            var bundlePath = Path.GetDirectoryName(Application.persistentDataPath) + "\\Reverie\\AvatarBundles";
            
            if (!Directory.Exists(bundlePath))
                Directory.CreateDirectory(bundlePath);
            
            BuildPipeline.BuildAssetBundles (
                Path.GetDirectoryName(Application.persistentDataPath) + "\\Reverie\\AvatarBundles",
                ab, 
                BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget
            );
            
            File.Delete(bundlePath + "\\" + avatarBundleName + ".rvav.manifest");
            File.Delete(bundlePath + "\\AvatarBundles");
            File.Delete(bundlePath + "\\AvatarBundles.manifest");
            File.Delete(Directory.GetCurrentDirectory() + "\\" + localPath);
            File.Delete(Directory.GetCurrentDirectory() + "\\" + localPath + ".meta");
            DestroyImmediate(clone);
            
            Debug.Log("Avatar Exported Successfully");
        }

        private void ReplaceDynamicBoneWithData(GameObject clone)
        {
            string scriptName = "DynamicBone";
            Type scriptType = Type.GetType(scriptName);
            
            if (scriptType == null) return;
            
            //Debug.Log("DynamicBone found in current assembly");
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!defines.Contains("DYNAMICBONE"))
            {
                defines += ";" + "DYNAMICBONE"; 
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
            }
            EditorUtility.RequestScriptReload();
            
            #if DYNAMICBONE
            foreach (var dynbonecol in clone.transform.GetComponentsInChildren<DynamicBoneCollider>())
            {
                var dyndata = dynbonecol.gameObject.AddComponent<DynamicBoneColliderData>();
                dyndata.m_Radius = dynbonecol.m_Radius;
                dyndata.m_Radius2 = dynbonecol.m_Radius2;
                dyndata.m_Center = dynbonecol.m_Center;
                dyndata.m_Height = dynbonecol.m_Height;
                dyndata.m_Direction = (DynamicBoneColliderBaseData.Direction)dynbonecol.m_Direction;
                dyndata.m_Bound = (DynamicBoneColliderBaseData.Bound)dynbonecol.m_Bound;
            }
            
            foreach (var dynbonepcol in clone.transform.GetComponentsInChildren<DynamicBonePlaneCollider>())
            {
                var dyndata = dynbonepcol.gameObject.AddComponent<DynamicBonePlaneColliderData>();

                dyndata.m_Center = dynbonepcol.m_Center;
                dyndata.m_Direction = (DynamicBoneColliderBaseData.Direction)dynbonepcol.m_Direction;
                dyndata.m_Bound = (DynamicBoneColliderBaseData.Bound)dynbonepcol.m_Bound;
            }
            
            foreach (var dynbone in clone.transform.GetComponentsInChildren<DynamicBone>())
            {
                var dyndata = dynbone.gameObject.AddComponent<DynamicBoneData>();
                
                dyndata.m_Colliders = new List<DynamicBoneColliderBaseData>();
                for (int i = 0; i < dynbone.m_Colliders.Count; i++)
                {
                    DynamicBoneColliderBaseData coldata = dynbone.m_Colliders[i].gameObject.GetComponent<DynamicBoneColliderBaseData>();
                    dyndata.m_Colliders.Add(coldata);
                }
                
                dyndata.m_Damping = dynbone.m_Damping;
                dyndata.m_DampingDistrib = dynbone.m_DampingDistrib;
                dyndata.m_DistanceToObject = dynbone.m_DistanceToObject;
                dyndata.m_DistantDisable = dynbone.m_DistantDisable;
                dyndata.m_Elasticity = dynbone.m_Elasticity;
                dyndata.m_ElasticityDistrib = dynbone.m_ElasticityDistrib;
                dyndata.m_EndLength = dynbone.m_EndLength;
                dyndata.m_EndOffset = dynbone.m_EndOffset;
                dyndata.m_Exclusions = dynbone.m_Exclusions;
                dyndata.m_Force = dynbone.m_Force;
                dyndata.m_FreezeAxis = (DynamicBoneData.FreezeAxis)dynbone.m_FreezeAxis;
                dyndata.m_Friction = dynbone.m_Friction;
                dyndata.m_FrictionDistrib = dynbone.m_FrictionDistrib;
                dyndata.m_Gravity = dynbone.m_Gravity;
                dyndata.m_Inert = dynbone.m_Inert;
                dyndata.m_InertDistrib = dynbone.m_InertDistrib;
                dyndata.m_Radius = dynbone.m_Radius;
                dyndata.m_RadiusDistrib = dynbone.m_RadiusDistrib;
                dyndata.m_Root = dynbone.m_Root;
                dyndata.m_Roots = dynbone.m_Roots;
                dyndata.m_Stiffness = dynbone.m_Stiffness;
                dyndata.m_StiffnessDistrib = dynbone.m_StiffnessDistrib;
                dyndata.m_UpdateMode = (DynamicBoneData.UpdateMode)dynbone.m_UpdateMode;
                dyndata.m_UpdateRate = dynbone.m_UpdateRate;
                dyndata.m_Multithread = dynbone.m_Multithread;
                
                DestroyImmediate(dynbone);
            }

            foreach (var col in clone.GetComponentsInChildren<DynamicBoneCollider>())
            {
                DestroyImmediate(col);
            }
            
            foreach (var col in clone.GetComponentsInChildren<DynamicBonePlaneCollider>())
            {
                DestroyImmediate(col);
            }
            #endif
        }

        public bool CheckForMissingScripts(GameObject clone)
        {
            var objs = clone.GetComponentsInChildren<Transform>(true).Select(x => x.gameObject).ToArray();
            int count = objs.Sum(GameObjectUtility.RemoveMonoBehavioursWithMissingScript);
            
            if (count == 0) return true;
            bool choice = EditorUtility.DisplayDialog("Avatar Build Warning",
                $"You have {count} missing scripts on your avatar and they will be removed. Continue?",
                "Remove and Continue",
                "Cancel Build"
            );
            return choice;
        }
        
        public static Transform[] GetTopLevelChildren(Transform parent)
        {
            List<Transform> children = new List<Transform>();
            foreach (var child in parent.GetComponentsInChildren<Transform>())
            {
                if (child.parent == parent)
                {
                    children.Add(child);
                }
            }
            return children.ToArray();
        }
        public void PreviewAngleLimit(EyeConstraintDirection dir)
        {
            switch(dir)
            {
                case EyeConstraintDirection.None:
                    leftEye.localEulerAngles = leftEyeForward;
                    rightEye.localEulerAngles = rightEyeForward;
                    break;
                case EyeConstraintDirection.Left:
                    leftEye.localEulerAngles = leftEyeForward + new Vector3(0, -angleLimitLeft, 0);
                    rightEye.localEulerAngles = rightEyeForward + new Vector3(0, -angleLimitLeft, 0);
                    break;
                case EyeConstraintDirection.Right:
                    leftEye.localEulerAngles = leftEyeForward + new Vector3(0, angleLimitRight, 0);
                    rightEye.localEulerAngles = rightEyeForward + new Vector3(0, angleLimitRight, 0);
                    break;
                case EyeConstraintDirection.Up:
                    leftEye.localEulerAngles = leftEyeForward + new Vector3(-angleLimitUp, 0, 0);
                    rightEye.localEulerAngles = rightEyeForward + new Vector3(-angleLimitUp, 0, 0);
                    break;
                case EyeConstraintDirection.Down:
                    leftEye.localEulerAngles = leftEyeForward + new Vector3(angleLimitDown, 0, 0);
                    rightEye.localEulerAngles = rightEyeForward + new Vector3(angleLimitDown, 0, 0);
                    break;
            }
        }
        
        #endif
    }
}
