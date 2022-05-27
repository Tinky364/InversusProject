using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Inversus.Manager;
using Inversus.Data;

using static Inversus.Facade;

namespace Inversus.UI.MainMenuScene
{
    public class PlayLocallyPanel : Panel
    {
        [SerializeField]
        private PlayerConnectionGrid _playerConnectionGrid1;
        [SerializeField]
        private PlayerConnectionGrid _playerConnectionGrid2;
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
            _mapIdDropdown.ClearOptions();
            _mapIdDropdown.AddOptions(new List<string> {"1", "2"});
            
            _victoryScoreDropdown.ClearOptions();
            _victoryScoreDropdown.AddOptions(new List<string> {"1", "2", "3", "5", "10"});
            
            _colorsDropdown.ClearOptions();
            _colorsDropdown.AddOptions(new List<string> {"White - Black", "Red - Blue"});
        }

        private void OnEnable()
        {
            SEventBus.InputProfileJoined.AddListener(OnInputProfileJoined);
            SEventBus.InputProfileLeft.AddListener(OnInputProfileLeft);

            _startGameButton.interactable = false;
        }
        
        private void OnDisable()
        {
            SEventBus.InputProfileJoined.RemoveListener(OnInputProfileJoined);
            SEventBus.InputProfileLeft.RemoveListener(OnInputProfileLeft);
            _playerConnectionGrid1.Hide();
            _playerConnectionGrid2.Hide();
        }

        private void OnInputProfileJoined(InputProfile inputProfile)
        {
            switch (inputProfile.Id)
            {
                case 1:
                    _playerConnectionGrid1.Display(
                        inputProfile.Name, inputProfile.PlayerInput.devices[0].displayName
                    );
                    SCanvasManager.SetSelectedGameObject(_mapIdDropdown.gameObject);
                    break;
                case 2:
                    _playerConnectionGrid2.Display(
                        inputProfile.Name, inputProfile.PlayerInput.devices[0].displayName
                    );
                    break;
            }

            if (SInputProfileManager.InputProfileCount >= 2) _startGameButton.interactable = true;
        }

        private void OnInputProfileLeft(InputProfile inputProfile)
        {
            switch (inputProfile.Id)
            {
                case 1: _playerConnectionGrid1.Hide();
                    break;
                case 2: _playerConnectionGrid2.Hide();
                    break;
            }

            if (SInputProfileManager.InputProfileCount < 2) _startGameButton.interactable = false;
        }

        public void EnableInputProfileManager(int maxPlayerCount)
        {
            SInputProfileManager.Enable(maxPlayerCount);
        }
        
        public void DisableInputProfileManager()
        {
            SInputProfileManager.Disable();
        }

        public void StartGameButton_Click(SceneData sceneData)
        {
            Debug.Log("StartGameRequested Event => Invoke()");

            if (int.TryParse(
                    _mapIdDropdown.options[_mapIdDropdown.value].text, out int startingMapId
                ) &&
                int.TryParse(
                    _victoryScoreDropdown.options[_victoryScoreDropdown.value].text, 
                    out int victoryScore
                ))
            {
                SEventBus.StartGameRequested?.Invoke(
                    startingMapId, victoryScore, _colorsDropdown.value + 1, GameType.Local
                );
            }
            else
            {
                Debug.LogWarning("Parsing Failed");
                SEventBus.StartGameRequested?.Invoke(1, 1, 1, GameType.Local);
            }
            
            SSceneCreator.LoadScene(sceneData, SubSceneLoadMode.Single);
        }
    }
}
