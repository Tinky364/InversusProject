using UnityEngine;

using Inversus.Manager;
using Inversus.UI;

namespace Inversus.Game
{
    public class MainMenuCanvasManager : CanvasManager
    {
        [Header("PANELS")]
        [SerializeField]
        private Panel MainMenuPanel;
        [SerializeField]
        private Panel PlayPanel;
        [SerializeField]
        private Panel PlayLocallyPanel;

        protected override void Awake()
        {
            base.Awake();
            
            MainMenuPanel.SetDisplay(true);
            PlayPanel.SetDisplay(false);
            PlayLocallyPanel.SetDisplay(false);
        }
    }
}

