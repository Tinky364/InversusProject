using UnityEngine;

using Inversus.Manager;
using Inversus.UI;

namespace Inversus.Game
{
    public class MainMenuCanvasManager : CanvasManager
    {
        [Header("PANELS")]
        [SerializeField]
        private Panel _mainMenuPanel;
        [SerializeField]
        private Panel _playPanel;
        [SerializeField]
        private Panel _playLocallyPanel;

        protected override void Awake()
        {
            base.Awake();
            
            _mainMenuPanel.SetDisplay(true);
            _playPanel.SetDisplay(false);
            _playLocallyPanel.SetDisplay(false);
        }
    }
}

