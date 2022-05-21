using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Inversus.Manager;
using Inversus.Data;

using static Inversus.Facade;

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
        [SerializeField]
        private TMP_Dropdown _mapIdDropdown;
        [SerializeField]
        private TMP_Dropdown _victoryScoreDropdown;
        [SerializeField]
        private TMP_Dropdown _colorsDropdown;

        private void Awake()
        {
            SEventBus.PlayerJoinedGame.AddListener(OnPlayerJoined);
            SEventBus.PlayerLeftGame.AddListener(OnPlayerLeft);
            
            _mapIdDropdown.ClearOptions();
            _mapIdDropdown.AddOptions(new List<string> {"1", "2"});
            
            _victoryScoreDropdown.ClearOptions();
            _victoryScoreDropdown.AddOptions(new List<string> {"1", "2", "3", "5", "10"});
            
            _colorsDropdown.ClearOptions();
            _colorsDropdown.AddOptions(new List<string> {"White - Black", "Red - Blue"});
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
            switch (player.Id)
            {
                case 1:
                    _playerInputGrid1.Display(
                        player.Name, player.PlayerInput.devices[0].displayName
                    );
                    break;
                case 2:
                    _playerInputGrid2.Display(
                        player.Name, player.PlayerInput.devices[0].displayName
                    );
                    break;
            }

            if (SInputManager.PlayersCount >= 2) _startGameButton.interactable = true;
        }

        private void OnPlayerLeft(Player player)
        {
            switch (player.Id)
            {
                case 1: 
                    _playerInputGrid1.Hide();
                    break;
                case 2: 
                    _playerInputGrid2.Hide();
                    break;
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

        public void OnStartGameButtonClick(SceneData sceneData)
        {
            Debug.Log("PlayLocallyStartGameButtonClicked Event => Invoke()");

            if (int.TryParse(
                    _mapIdDropdown.options[_mapIdDropdown.value].text, out int startingMapId
                ) &&
                int.TryParse(
                    _victoryScoreDropdown.options[_victoryScoreDropdown.value].text, 
                    out int victoryScore
                ))
            {
                SEventBus.PlayLocallyStartGameButtonClicked?.Invoke(
                    startingMapId, victoryScore, _colorsDropdown.value + 1
                );
            }
            else
            {
                Debug.LogWarning("Parsing Failed");
                SEventBus.PlayLocallyStartGameButtonClicked?.Invoke(1, 1, 1);
            }
            
            SSceneCreator.LoadScene(sceneData, SubSceneLoadMode.Single);
        }
    }
}
