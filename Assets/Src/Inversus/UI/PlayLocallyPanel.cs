using UnityEngine;

using Inversus.Manager;
using UnityEngine.UI;
using static Inversus.Manager.ManagerFacade;

namespace Inversus.UI
{
    public class PlayLocallyPanel : Panel
    {
        [SerializeField]
        private PlayerInputGrid _playerInputGrid1;
        [SerializeField]
        private PlayerInputGrid _playerInputGrid2;
        [SerializeField]
        private Button _startGameButton;

        private void Awake()
        {
            SEventBus.PlayerJoinedGame.AddListener(OnPlayerJoined);
            SEventBus.PlayerLeftGame.AddListener(OnPlayerLeft);
            
        }

        private void OnEnable()
        {
            _startGameButton.interactable = SInputManager.PlayersCount >= 2;
        }

        private void OnDestroy()
        {
            SEventBus.PlayerJoinedGame.RemoveListener(OnPlayerJoined);
            SEventBus.PlayerLeftGame.RemoveListener(OnPlayerLeft);
        }

        private void OnPlayerJoined(Player player)
        {
            if (player.Id == 1)
            {
                _playerInputGrid1.Set(player.Name);
            }

            if (player.Id == 2)
            {
                _playerInputGrid2.Set(player.Name);
            }

            if (SInputManager.PlayersCount >= 2) _startGameButton.interactable = true;
        }

        private void OnPlayerLeft(Player player)
        {
            if (player.Id == 1)
            {
                _playerInputGrid1.Set("");
            }

            if (player.Id == 2)
            {
                _playerInputGrid2.Set("");
            }
            
            if (SInputManager.PlayersCount < 2) _startGameButton.interactable = false;
        }

        public void SetStatePlayLocallyMenu()
        {
            SMainManager.State = States.PlayLocallyMenu;
        }

        public void SetStateMainMenu()
        {
            SMainManager.State = States.MainMenu;
        }

        public void RemoveAllLocallyPlayers()
        {
            SInputManager.RemoveAllPlayers();
        }
    }
}
