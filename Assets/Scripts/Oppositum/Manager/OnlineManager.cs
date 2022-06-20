using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using static Oppositum.Facade;

namespace Oppositum.Manager
{
    public class OnlineManager : MonoBehaviourPunCallbacks
    {
        public static OnlineManager Instance { get; private set; }

        private string _nickName;
        public string NickName
        {
            get => _nickName;
            set
            {
                _nickName = value;
                PlayerPrefs.SetString("NickName", _nickName);
                PhotonNetwork.NickName = _nickName;
            }
        }

        private bool _isConnected;
        public bool IsConnected => _isConnected && PhotonNetwork.IsConnectedAndReady;
        
        private bool _inLobby;
        public bool InLobby => _inLobby && PhotonNetwork.InLobby;

        private bool _inRoom;
        public bool InRoom => _inRoom && PhotonNetwork.InRoom;

        private List<RoomInfo> _roomList;
        public List<RoomInfo> RoomList => _roomList;

        private void SetSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("An instance of this singleton already exists.");
                Debug.LogWarning($"Destroyed Component => {this}");
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
        
        private void Awake()
        {
            SetSingleton();

            NickName = PlayerPrefs.GetString("NickName", $"User{Random.Range(0, 99999)}");
           
            SEventBus.ServerConnectionRequested.AddListener(ConnectToServer);
            SEventBus.JoinLobbyRequested.AddListener(JoinLobby);
            SEventBus.LeaveLobbyRequested.AddListener(LeaveLobby);
            SEventBus.CreateRoomRequested.AddListener(CreateRoom);
            SEventBus.JoinRoomRequested.AddListener(JoinRoom);
            SEventBus.LeaveRoomRequested.AddListener(LeaveRoom);
        }
        
        private void OnDestroy()
        {
            SEventBus.ServerConnectionRequested.RemoveListener(ConnectToServer);
            SEventBus.JoinLobbyRequested.RemoveListener(JoinLobby);
            SEventBus.LeaveLobbyRequested.RemoveListener(LeaveLobby);
            SEventBus.CreateRoomRequested.RemoveListener(CreateRoom);
            SEventBus.JoinRoomRequested.RemoveListener(JoinRoom);
            SEventBus.LeaveRoomRequested.RemoveListener(LeaveRoom);
        }

        private void ConnectToServer()
        {
            if (IsConnected) return;

            PhotonNetwork.NickName = NickName;
            PhotonNetwork.ConnectUsingSettings();
        }

        private void JoinLobby()
        {
            if (!IsConnected) return;

            PhotonNetwork.JoinLobby();
        }

        private void LeaveLobby()
        {
            PhotonNetwork.LeaveLobby();
        }

        private void CreateRoom(string roomName, int maxPlayers)
        {
            if (!IsConnected) return;

            PhotonNetwork.CreateRoom(
                $"{PhotonNetwork.NickName}'s room", CreateRoomOptions((byte)maxPlayers)
            );
        }

        private void JoinRoom(string roomName)
        {
            if (!IsConnected) return;

            PhotonNetwork.JoinRoom(roomName);
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private RoomOptions CreateRoomOptions(byte maxPlayers)
        {
            RoomOptions roomOptions = new()
            {
                MaxPlayers = maxPlayers, IsOpen = true, IsVisible = true
            };
            return roomOptions;
        }
        
#region Callbacks
        public override void OnConnectedToMaster()
        {
            Debug.Log("Client connected to the server.");

            _isConnected = true;
            SEventBus.ServerConnected?.Invoke();
        }
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Client disconnected from the server.");
            
            _isConnected = false;
            _inLobby = false;
            _inRoom = false;
            SEventBus.ServerDisconnected?.Invoke();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Client joined the lobby.");

            _inLobby = true;
            SEventBus.LobbyJoined?.Invoke();
        }

        public override void OnLeftLobby()
        {
            Debug.Log("Client left the lobby.");

            _inLobby = false;
            _inRoom = false;
            SEventBus.LobbyLeft?.Invoke();
        }

        public override void OnCreatedRoom()
        {
            Debug.Log(
                $"Client named {PhotonNetwork.NickName} created the room named" +
                $" {PhotonNetwork.CurrentRoom.Name}."
            );

            _inRoom = true;
            SEventBus.RoomCreated?.Invoke(PhotonNetwork.CurrentRoom.Name);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Client could not create the room. {message}");

            _inRoom = false;
            SEventBus.RoomCreateFailed?.Invoke();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Client joined the room named {PhotonNetwork.CurrentRoom.Name}.");
            
            _inRoom = true;
            SEventBus.RoomJoined?.Invoke();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Client could not join the room. {message}");
            
            _inRoom = false;
            SEventBus.RoomJoinFailed?.Invoke();
        }
        
        public override void OnLeftRoom()
        {
            Debug.Log("Client left the room.");

            _inRoom = false;
            SEventBus.RoomLeft?.Invoke();
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"Player named {newPlayer.NickName} entered the room.");
            
            SEventBus.PlayerEnteredRoom?.Invoke(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"Player named {otherPlayer.NickName} left the room.");
            
            SEventBus.PlayerLeftRoom?.Invoke(otherPlayer);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log(
                $"Master client left the room. New master client name is {newMasterClient.NickName}"
            );
            
            SEventBus.MasterClientSwitched?.Invoke(newMasterClient);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if (SMainManager.State != States.PlayOnlineMenu) return;
            _roomList = roomList;
        }
#endregion
    }
}
