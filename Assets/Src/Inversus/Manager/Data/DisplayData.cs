using System;
using UnityEngine;

namespace Inversus.Manager.Data
{
    [Serializable]
    public class DisplayData
    {
        [SerializeField] private Vector2Int _resolution = new Vector2Int(1600, 900);
        [SerializeField] private FullScreenMode _fullScreenMode = FullScreenMode.FullScreenWindow;

        public Vector2Int Resolution => _resolution;
        public FullScreenMode FullScreenMode => _fullScreenMode;
    }
}
