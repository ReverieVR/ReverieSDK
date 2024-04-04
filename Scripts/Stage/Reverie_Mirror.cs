using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReverieSDK
{
    public class Reverie_Mirror : MonoBehaviour
    {
        public enum MirrorOrientations {
            XPlus, YPlus, ZPlus, XMinus, YMinus, ZMinus
        }

        public enum MirrorTextureQuality {
            Auto, VeryHigh, High, Medium, Low, VeryLow
        }

        public bool disablePixelLights = true;
        public MirrorTextureQuality reflectionQuality = MirrorTextureQuality.Medium;
        public float clipPlaneOffset = 0f;
        public MirrorOrientations orientation = MirrorOrientations.ZMinus;

        public LayerMask reflectLayers = -1;
    }
}