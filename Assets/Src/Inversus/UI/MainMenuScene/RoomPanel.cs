using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Inversus.Facade;

namespace Inversus.UI
{
    public class RoomPanel : Panel
    {
        [SerializeField]
        private Button _startGameButton;
        [SerializeField]
        private TMP_Dropdown _mapIdDropdown;
        [SerializeField]
        private TMP_Dropdown _victoryScoreDropdown;
        [SerializeField]
        private TMP_Dropdown _colorsDropdown;
        [SerializeField]
        private Button _backButton;

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
            SEventBus.ServerDisconnected.AddListener(OnServerDisconnected);
            SEventBus.RoomLeft.AddListener(OnRoomLeft);

            SetUiElementsInteractable(SOnlineGameManager.IsMasterClient);
        }

        private void OnDisable()
        {
            SEventBus.ServerDisconnected.RemoveListener(OnServerDisconnected);
            SEventBus.RoomLeft.RemoveListener(OnRoomLeft);
        }
        
        private void OnServerDisconnected()
        {
            if (SMainManager.State != States.PlayOnlineMenu) return;
            
            SMainMenuCanvasManager.PlayPanel.SetDisplay(true);
            SetDisplay(false);
        }

        public void BackButton_Click()
        {
            SCanvasManager.SetUiInput(false);
            SEventBus.LeaveRoomRequested?.Invoke();
        }

        private void OnRoomLeft()
        {
            SMainMenuCanvasManager.PlayOnlinePanel.SetDisplay(true);
            SetDisplay(false);
            SCanvasManager.SetUiInput(true);
        }

        private void SetUiElementsInteractable(bool value)
        {
            _startGameButton.interactable = value;
            _mapIdDropdown.interactable = value;
            _victoryScoreDropdown.interactable = value;
            _colorsDropdown.interactable = value;
        }
    }
}
