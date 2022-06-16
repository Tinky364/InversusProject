using UnityEngine;
using Oppositum.Manager;
using Oppositum.UI.GameScene;

namespace Oppositum.UI
{
    public class GameCanvasManager : CanvasManager
    {
        [Header("PANELS")]
        [SerializeField]
        private ReadyPanel _readyPanel;
        [SerializeField]
        private PausePanel _pausePanel;
        [SerializeField]
        private RoundEndPanel _roundEndPanel;
        [SerializeField]
        private GameEndPanel _gameEndPanel;

        public ReadyPanel ReadyPanel => _readyPanel;
        public PausePanel PausePanel => _pausePanel;
        public RoundEndPanel RoundEndPanel => _roundEndPanel;
        public GameEndPanel GameEndPanel => _gameEndPanel;
        
        protected override void Awake()
        {
            base.Awake();
            
            BackgroundPanel.SetDisplay(false);
            ForegroundPanel.SetDisplay(true);
            ReadyPanel.SetDisplay(false);
            PausePanel.SetDisplay(false);
            RoundEndPanel.SetDisplay(false);
            GameEndPanel.SetDisplay(false);
        }
    }
}

