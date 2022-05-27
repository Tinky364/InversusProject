using UnityEngine;

using Inversus.Manager;
using Inversus.UI.GameScene;

namespace Inversus.UI
{
    public class GameCanvasManager : CanvasManager
    {
        [Header("PANELS")]
        [SerializeField]
        private PausePanel _pausePanel;
        [SerializeField]
        private RoundEndPanel _roundEndPanel;
        [SerializeField]
        private GameEndPanel _gameEndPanel;

        public PausePanel PausePanel => _pausePanel;
        public RoundEndPanel RoundEndPanel => _roundEndPanel;
        public GameEndPanel GameEndPanel => _gameEndPanel;
        
        protected override void Awake()
        {
            base.Awake();
            
            BackgroundPanel.SetDisplay(false);
            ForegroundPanel.SetDisplay(false);
            PausePanel.SetDisplay(false);
            RoundEndPanel.SetDisplay(false);
            GameEndPanel.SetDisplay(false);
        }
    }
}

