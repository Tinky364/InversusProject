using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

using Inversus.Data;
using Inversus.Manager;

using static Inversus.Facade;

namespace Inversus.UI
{
    public class RoomPanel : Panel
    {
        [SerializeField]
        private TextMeshProUGUI _playerCountText;
        [SerializeField]
        private PlayerConnectionGrid[] _playerConnectionGrids;
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

        private PhotonView _photonView;
        private int _playerCount;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            
            _mapIdDropdown.ClearOptions();
            _victoryScoreDropdown.ClearOptions();
            _colorsDropdown.ClearOptions();

            _mapIdDropdown.AddOptions(new List<string> {"1", "2"});
            _victoryScoreDropdown.AddOptions(new List<string> {"1", "2", "3", "5", "10"});
            _colorsDropdown.AddOptions(new List<string> {"White - Black", "Red - Blue"});
        }

        private void OnEnable()
        {
            SEventBus.ServerDisconnected.AddListener(OnServerDisconnected);
            SEventBus.RoomLeft.AddListener(OnRoomLeft);
            SEventBus.InputProfileJoined.AddListener(OnInputProfileJoined);
            SEventBus.InputProfileLeft.AddListener(OnInputProfileLeft);
            SEventBus.MasterClientSwitched.AddListener(OnMasterClientSwitched);
            
            _mapIdDropdown.onValueChanged.AddListener(MapIdDropdown_ValueChange);
            _victoryScoreDropdown.onValueChanged.AddListener(VictoryScoreDropdown_ValueChange);
            _colorsDropdown.onValueChanged.AddListener(ColorsDropdown_ValueChange);
           
            SetUiElementsInteractable(PhotonNetwork.IsMasterClient);
            _startGameButton.interactable = false;
        }

        private void OnDisable()
        {
            SEventBus.ServerDisconnected.RemoveListener(OnServerDisconnected);
            SEventBus.RoomLeft.RemoveListener(OnRoomLeft);
            SEventBus.InputProfileJoined.RemoveListener(OnInputProfileJoined);
            SEventBus.InputProfileLeft.RemoveListener(OnInputProfileLeft);
            SEventBus.MasterClientSwitched.RemoveListener(OnMasterClientSwitched);

            _mapIdDropdown.onValueChanged.RemoveListener(MapIdDropdown_ValueChange);
            _victoryScoreDropdown.onValueChanged.RemoveListener(VictoryScoreDropdown_ValueChange);
            _colorsDropdown.onValueChanged.RemoveListener(ColorsDropdown_ValueChange);
            
            _playerConnectionGrids[0].Hide();
            _playerConnectionGrids[1].Hide();
        }
        
        private void Update()
        {
            if (_playerCount != PhotonNetwork.CurrentRoom.PlayerCount)
            {
                _playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
                _playerCountText.SetText(
                    $"Player Count: {_playerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}"
                );
            }
        }
        
        public void EnableInputProfileManager(int maxPlayerCount)
        {
            SInputProfileManager.Enable(maxPlayerCount);
        }
        
        private void SetUiElementsInteractable(bool value)
        {
            _startGameButton.interactable = value;
            _mapIdDropdown.interactable = value;
            _victoryScoreDropdown.interactable = value;
            _colorsDropdown.interactable = value;
        }
       
#region CALLBACKS
        private void OnInputProfileJoined(InputProfile inputProfile)
        {
            _photonView.RPC(
                "PlayerConnectionGridDisplay", RpcTarget.AllBuffered,
                PhotonNetwork.IsMasterClient ? 0 : 1,
                PhotonNetwork.LocalPlayer.NickName,
                inputProfile.PlayerInput.devices[0].displayName
            );
        }
        
        [PunRPC]
        public void PlayerConnectionGridDisplay(int id, string username, string deviceName)
        {
            _playerConnectionGrids[id].Display(username, deviceName);
            SCanvasManager.SetSelectedGameObject(
                PhotonNetwork.IsMasterClient
                    ? _mapIdDropdown.gameObject
                    : _backButton.gameObject
            );
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && PhotonNetwork.IsMasterClient)
                _startGameButton.interactable = true;
        }

        private void OnInputProfileLeft(InputProfile inputProfile)
        {
            _photonView.RPC(
                "PlayerConnectionGridHide", RpcTarget.AllBuffered,
                PhotonNetwork.IsMasterClient ? 0 : 1
            );
        }

        [PunRPC]
        public void PlayerConnectionGridHide(int id)
        { 
            _playerConnectionGrids[id].Hide();
            
            if (PhotonNetwork.CurrentRoom.PlayerCount < 2 && PhotonNetwork.IsMasterClient)
                _startGameButton.interactable = false;
        }

        private void OnMasterClientSwitched(Player newMasterClient)
        {
            SInputProfileManager.RemoveAllInputProfiles();
            SCanvasManager.SetUiInput(false);
            SEventBus.LeaveRoomRequested?.Invoke();
        }
        
        private void OnServerDisconnected()
        {
            if (SMainManager.State != States.PlayOnlineMenu) return;
            
            SInputProfileManager.RemoveAllInputProfiles();
            SMainMenuCanvasManager.PlayPanel.SetDisplay(true);
            SetDisplay(false);
            SInputProfileManager.InstantiateDefaultInputProfile();
        }

        private void OnRoomLeft()
        {
            SCanvasManager.SetUiInput(true);
            SMainMenuCanvasManager.PlayOnlinePanel.SetDisplay(true);
            SetDisplay(false);
            SInputProfileManager.InstantiateDefaultInputProfile();
        }
#endregion

#region DROPDOWNS
        private void MapIdDropdown_ValueChange(int value)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            _photonView.RPC("MapIdDropdownUpdate", RpcTarget.OthersBuffered, value);
        }
        
        [PunRPC]
        public void MapIdDropdownUpdate(int value)
        {
            _mapIdDropdown.value = value;
        }
        
        private void VictoryScoreDropdown_ValueChange(int value)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            _photonView.RPC("VictoryScoreDropdownUpdate", RpcTarget.OthersBuffered, value);
        }
        
        [PunRPC]
        public void VictoryScoreDropdownUpdate(int value)
        {
            _victoryScoreDropdown.value = value;
        }
        
        private void ColorsDropdown_ValueChange(int value)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            _photonView.RPC("ColorsDropdownUpdate", RpcTarget.OthersBuffered, value);
        }
        
        [PunRPC]
        public void ColorsDropdownUpdate(int value)
        {
            _colorsDropdown.value = value;
        }
#endregion

#region BUTTONS
        public void StartGameButton_Click(SceneData sceneData)
        {
            _photonView.RPC("LoadScene", RpcTarget.All, sceneData.Name);
        }

        [PunRPC]
        public void LoadScene(string sceneName)
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
                    startingMapId, victoryScore, _colorsDropdown.value + 1, GameType.Online
                );
            }
            else
            {
                Debug.LogWarning("Parsing Failed");
                SEventBus.StartGameRequested?.Invoke(1, 1, 1, GameType.Online);
            }
            
            SSceneCreator.LoadScene(sceneName, SubSceneLoadMode.Single);
        }
        
        public void BackButton_Click()
        {
            SInputProfileManager.RemoveAllInputProfiles();
            SCanvasManager.SetUiInput(false);
            SEventBus.LeaveRoomRequested?.Invoke();
        }
#endregion
    }
}
