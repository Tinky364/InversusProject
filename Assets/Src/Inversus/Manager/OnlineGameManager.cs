using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using static Inversus.Facade;

namespace Inversus.Manager
{
    public class OnlineGameManager : MonoBehaviourPunCallbacks
    {
        public static OnlineGameManager Instance { get; private set; }

        private bool _isConnected;
        public bool IsConnected => _isConnected && PhotonNetwork.IsConnectedAndReady;

        private bool _inRoom;
        public bool InRoom => _inRoom && PhotonNetwork.InRoom;

        public bool IsMasterClient => PhotonNetwork.IsMasterClient;
        
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
            
            SEventBus.ServerConnectionRequested.AddListener(ConnectToServer);
            SEventBus.CreateRoomRequested.AddListener(CreateRoom);
            SEventBus.JoinRoomRequested.AddListener(JoinRoom);
            SEventBus.LeaveRoomRequested.AddListener(LeaveRoom);
        }

        private void OnDestroy()
        {
            SEventBus.ServerConnectionRequested.RemoveListener(ConnectToServer);
            SEventBus.CreateRoomRequested.RemoveListener(CreateRoom);
            SEventBus.JoinRoomRequested.RemoveListener(JoinRoom);
            SEventBus.LeaveRoomRequested.RemoveListener(LeaveRoom);
        }

        public void ConnectToServer()
        {
            if (PhotonNetwork.IsConnected) return;
            
            PhotonNetwork.ConnectUsingSettings();
        }

        public void JoinLobby()
        {
            PhotonNetwork.JoinLobby();
        }

        public void CreateRoom(string roomName, int maxPlayers)
        {
            PhotonNetwork.CreateRoom(null, CreateRoomOptions((byte)maxPlayers));
        }

        public void JoinRoom()
        {
            //PhotonNetwork.JoinRoom(roomName);
            PhotonNetwork.JoinRandomRoom();
        }

        public void LeaveRoom()
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
            Debug.Log("Connect to the server: Success");

            JoinLobby();
        }
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Disconnected from the server");
            
            _isConnected = false;
            SEventBus.ServerDisconnected?.Invoke();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("Join the lobby: Success");
            
            _isConnected = true;
            SEventBus.ServerConnected?.Invoke();
        }
        
        public override void OnCreatedRoom()
        {
            Debug.Log($"Create the room named {PhotonNetwork.CurrentRoom.Name}: Success");

            _inRoom = true;
            SEventBus.RoomCreated?.Invoke(PhotonNetwork.CurrentRoom.Name);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Create the room: Fail, {message}");

            _inRoom = false;
            SEventBus.RoomCreateFailed?.Invoke();
        }

        public override void OnLeftRoom()
        {
            Debug.Log("Left the room");

            _inRoom = false;
            SEventBus.RoomLeft?.Invoke();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Join the room named {PhotonNetwork.CurrentRoom.Name}: Success");
            
            _inRoom = true;
            SEventBus.RoomJoined?.Invoke();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Join the room: Fail");
            
            _inRoom = false;
            SEventBus.RoomJoinFailed?.Invoke();
        }
#endregion
    }
}
