using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ReverieSDK
{
    public class Reverie_VideoPlayer : MonoBehaviour
    {
        public float playerVolume = 0.2f;
        public bool randomTimeStart = false;
        public RenderTexture videoTexture;
        public bool shuffleVideos = false;
        
        public List<string> videoUrls = new List<string>(1);

        public event Action PlayVideoUpdated;
        public event Action PauseVideoUpdated;
        public event Action StopVideoUpdated;
        public event Action NextVideoUpdated;
        public event Action PreviousVideoUpdated;
        public event Action<float> ChangeTimeUpdated;
        public event Action<float> ChangeVolumeUpdated;
        
        public void PlayVideo() { PlayVideoUpdated?.Invoke(); }
        public void PauseVideo() { PauseVideoUpdated?.Invoke(); }
        public void StopVideo() { StopVideoUpdated?.Invoke(); }
        public void NextVideo() { NextVideoUpdated?.Invoke(); }
        public void PreviousVideo() { PreviousVideoUpdated?.Invoke(); }
        public void ChangeTime(float time) { ChangeTimeUpdated?.Invoke(time); }
        public void ChangeVolume(float volume) { ChangeVolumeUpdated?.Invoke(volume); }
    }
}