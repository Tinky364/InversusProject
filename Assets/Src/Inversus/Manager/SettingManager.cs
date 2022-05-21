using System;
using UnityEngine;

using Inversus.Helper;
using Inversus.Data;

namespace Inversus.Manager
{
    public class SettingManager : SingletonMonoBehaviour<SettingManager>
    {
        [SerializeField] 
        private FrameRate _frameRate = FrameRate.Default;
        [SerializeField] 
        private DisplayData displayData;
        
        protected override void Awake()
        {
            base.Awake();
            
            ChangeFrameRate(_frameRate);
            Screen.SetResolution(
                displayData.Resolution.x, displayData.Resolution.y, displayData.FullScreenMode
            );
        }

        private void ChangeFrameRate(FrameRate newFrameRate)
        {
            switch (newFrameRate)
            {
                case FrameRate.Default:
                    SetFrameRate();
                    break;
                case FrameRate.Rate30:
                case FrameRate.Rate45:
                case FrameRate.Rate60:
                case FrameRate.Rate100:
                case FrameRate.Rate120:
                    SetFrameRate((int)newFrameRate);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.Log($"New Frame Rate: {newFrameRate}");
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
