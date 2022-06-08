using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

using static Inversus.Facade;

namespace Inversus.UI.MainMenuScene
{
    public class RoomListPanel : Panel
    {
        [SerializeField]
        private RoomTemplate _prefabRoomTemplate;
        [SerializeField]
        private Transform _rooms;
        [SerializeField]
        private Button _backButton;

        public Button BackButton => _backButton;
        public RoomInfo SelectedRoom => _selectedRoom;

        private Dictionary<RoomInfo, RoomTemplate> _roomsDict;
        private RoomInfo _selectedRoom;
        private bool _isRefreshButtonClicked = false;
        
        private void Awake()
        {
            _roomsDict = new Dictionary<RoomInfo, RoomTemplate>();
        }

        private void OnEnable()
        {
            SEventBus.ServerDisconnected.AddListener(OnServerDisconnected);
            SEventBus.LobbyLeft.AddListener(OnLobbyLeft);
            SEventBus.LobbyJoined.AddListener(OnLobbyJoined);
            
            CreateRoomList();
        }

        private void OnDisable()
        {
            SEventBus.ServerDisconnected.RemoveListener(OnServerDisconnected);
            SEventBus.LobbyLeft.RemoveListener(OnLobbyLeft);
            SEventBus.LobbyJoined.RemoveListener(OnLobbyJoined);
            
            ClearRoomList();
        }
        
        private void OnServerDisconnected()
        {
            if (SMainManager.State != States.PlayOnlineMenu) return;
            
            SMainMenuCanvasManager.PlayPanel.SetDisplay(true);
            SetDisplay(false);
        }

        private void CreateRoomList()
        {
            if (SOnlineManager.RoomList == null) return;
            
            foreach (RoomInfo roomInfo in SOnlineManager.RoomList)
            {
                CreateRoomTemplate(roomInfo);
            }
        }
        
        private void CreateRoomTemplate(RoomInfo roomInfo)
        {
            RoomTemplate roomTemplate = Instantiate(_prefabRoomTemplate, _rooms);
            roomTemplate.Initialize(roomInfo, this);
            _roomsDict.Add(roomInfo, roomTemplate);
        }

        private void ClearRoomList()
        {
            foreach (var pair in _roomsDict)
            {
                Destroy(pair.Value.gameObject);
            }
            _roomsDict.Clear();
        }

        public void JoinRoom_Click(RoomInfo roomInfo)
        {
            _selectedRoom = roomInfo;
            StartCoroutine(JoinRoom_ClickCor());
        }
        
        private IEnumerator JoinRoom_ClickCor()
        {
            SCanvasManager.SetUiInput(false);
            
            SEventBus.JoinRoomRequested?.Invoke(_selectedRoom.Name);
            float duration = 0f;
            while (duration < 5f)
            {
                if (SOnlineManager.InRoom)
                {
                    SCanvasManager.SetUiInput(true);
                    SMainMenuCanvasManager.RoomPanel.SetDisplay(true);
                    SetDisplay(false);
                    yield break;
                }
                duration += Time.deltaTime;
                yield return null;
            }
            
            SCanvasManager.SetUiInput(true);
        }

        public void OnSelectedRoomDestroyed()
        {
            _selectedRoom = null;
            SMainMenuCanvasManager.SetSelectedGameObject(_backButton.gameObject);
        }
        
        public void BackButton_Click()
        {
            SCanvasManager.SetUiInput(false);
            SEventBus.LeaveLobbyRequested?.Invoke();
        }

        public void RefreshButton_Click()
        {
            _isRefreshButtonClicked = true;
            SCanvasManager.SetUiInput(false);
            ClearRoomList();
            SEventBus.LeaveLobbyRequested?.Invoke();
        }

        private void OnLobbyLeft()
        {
            if (_isRefreshButtonClicked)
            {
                SEventBus.JoinLobbyRequested?.Invoke();
            }
            else
            {
                SCanvasManager.SetUiInput(true);
                SMainMenuCanvasManager.PlayOnlinePanel.SetDisplay(true);
                SetDisplay(false);
            }
        }

        private void OnLobbyJoined()
        {
            if (!_isRefreshButtonClicked) return;
            _isRefreshButtonClicked = false;

            CreateRoomList();
            SCanvasManager.SetUiInput(true);
        }
    }
}
