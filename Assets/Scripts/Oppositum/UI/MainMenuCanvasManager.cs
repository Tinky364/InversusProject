using UnityEngine;
using Oppositum.Manager;
using Oppositum.UI.MainMenuScene;
using static Oppositum.Facade;

namespace Oppositum.UI
{
    public class MainMenuCanvasManager : CanvasManager
    {
        [Header("PANELS")]
        [SerializeField]
        private Panel _mainMenuPanel;
        [SerializeField]
        private OptionsPanel _optionsPanel;
        [SerializeField]
        private Panel _creditsPanel;
        [SerializeField]
        private PlayPanel _playPanel;
        [SerializeField]
        private PlayLocallyPanel _playLocallyPanel;
        [SerializeField]
        private PlayOnlinePanel _playOnlinePanel;
        [SerializeField]
        private RoomPanel _roomPanel;
        [SerializeField]
        private RoomListPanel _roomListPanel;
        
        public Panel MainMenuPanel => _mainMenuPanel;
        public OptionsPanel OptionsPanel => _optionsPanel;
        public Panel CreditsPanel => _creditsPanel;
        public PlayPanel PlayPanel => _playPanel;
        public PlayLocallyPanel PlayLocallyPanel => _playLocallyPanel;
        public PlayOnlinePanel PlayOnlinePanel => _playOnlinePanel;
        public RoomPanel RoomPanel => _roomPanel;
        public RoomListPanel RoomListPanel => _roomListPanel;
        
        protected override void Awake()
        {
            base.Awake();
            
            BackgroundPanel.SetDisplay(true);
            ForegroundPanel.SetDisplay(false);
            OptionsPanel.SetDisplay(false);
            CreditsPanel.SetDisplay(false);
            PlayPanel.SetDisplay(false);
            PlayLocallyPanel.SetDisplay(false);
            PlayOnlinePanel.SetDisplay(false);
            RoomPanel.SetDisplay(false);
            RoomListPanel.SetDisplay(false);
            MainMenuPanel.SetDisplay(true);
        }
        
        public void SetStatePlayLocallyMenu()
        {
            SMainManager.State = States.PlayLocallyMenu;
        }

        public void SetStateMainMenu()
        {
            SMainManager.State = States.MainMenu;
        }
        
        public void SetStatePlayOnlineMenu()
        {
            SMainManager.State = States.PlayOnlineMenu;
        }
    }
}

