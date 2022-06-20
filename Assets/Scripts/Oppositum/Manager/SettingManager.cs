using System;
using UnityEngine;
using Oppositum.Helper;
using static Oppositum.Facade;

namespace Oppositum.Manager
{
    public class SettingManager : SingletonMonoBehaviour<SettingManager>
    {
        [SerializeField] 
        private FrameRate _frameRate = FrameRate.Default;
        [SerializeField, Min(0)] 
        private int _resolutionIndex = 0;
        [SerializeField, Min(0)] 
        private int _displayModeIndex = 0;
        [SerializeField, Range(0,1)]
        private float _volume = 1;

        public int ResolutionIndex => _resolutionIndex;
        public int DisplayModeIndex => _displayModeIndex;
        
        private Vector2Int _resolution;
        private FullScreenMode _displayMode;

        public float Volume
        {
            get => _volume;
            private set
            {
                _volume = value switch
                {
                    > 1 => 1,
                    < 0 => 0,
                    _ => value
                };
            }
        }

        protected override void Awake()
        {
            base.Awake();

            Volume = PlayerPrefs.GetFloat("Volume", 1);
            _resolutionIndex = PlayerPrefs.GetInt("Resolution", 0);
            _displayModeIndex = PlayerPrefs.GetInt("DisplayMode", 0);
            SetVolume(Volume);
            SetResolution(_resolutionIndex);
            SetDisplayMode(_displayModeIndex);
            SetFrameRate(_frameRate);
        }
        
        public void SetVolume(float volume)
        {
            Volume = volume;
            AudioListener.volume = volume;
            PlayerPrefs.SetFloat("Volume", Volume);
        }

        public void SetResolution(int index)
        {
            _resolutionIndex = index;
            _resolution =  SDatabase.GetResolution(_resolutionIndex);
            Screen.SetResolution(_resolution.x, _resolution.y, _displayMode);
            PlayerPrefs.SetInt("Resolution", _resolutionIndex);
        }

        public void SetDisplayMode(int index)
        {
            _displayModeIndex = index;
            _displayMode = SDatabase.GetDisplayMode(_displayModeIndex);
            Screen.SetResolution(_resolution.x, _resolution.y, _displayMode);
            PlayerPrefs.SetInt("DisplayMode", _displayModeIndex);
        }

        private void SetFrameRate(FrameRate frameRate)
        {
            switch (frameRate)
            {
                case FrameRate.Default:
                    SetFrameRate();
                    break;
                case FrameRate.Rate30:
                case FrameRate.Rate45:
                case FrameRate.Rate60:
                case FrameRate.Rate100:
                case FrameRate.Rate120:
                    SetFrameRate((int)frameRate);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.Log($"New Frame Rate: {frameRate}");
        }

        private static void SetFrameRate()
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1;
            Time.fixedDeltaTime = 1f / 60f;
        }
        
        private static void SetFrameRate(int newFrameRate)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = newFrameRate;
            Time.fixedDeltaTime = 1f / newFrameRate;
        }
    }
}
