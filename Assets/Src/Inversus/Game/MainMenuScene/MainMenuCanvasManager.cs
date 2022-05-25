using UnityEngine;

using Inversus.Manager;
using Inversus.UI;

using static Inversus.Facade;

namespace Inversus.Game
{
    public class MainMenuCanvasManager : CanvasManager
    {
        [Header("PANELS")]
        [SerializeField]
        private Panel _mainMenuPanel;
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
        public PlayPanel PlayPanel => _playPanel;
        public PlayLocallyPanel PlayLocallyPanel => _playLocallyPanel;
        public PlayOnlinePanel PlayOnlinePanel => _playOnlinePanel;
        public RoomPanel RoomPanel => _roomPanel;
        public RoomListPanel RoomListPanel => _roomListPanel;
        
        protected override void Awake()
        {
            base.Awake();
            
            _foregroundPanel.SetDisplay(false);
            MainMenuPanel.SetDisplay(true);
            PlayPanel.SetDisplay(false);
            PlayLocallyPanel.SetDisplay(false);
            PlayOnlinePanel.SetDisplay(false);
            RoomPanel.SetDisplay(false);
            RoomListPanel.SetDisplay(false);
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

