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
            if (IsConnected) return;

            PhotonNetwork.NickName = $"User{Random.Range(0, 9999)}";
            PhotonNetwork.ConnectUsingSettings();
        }

        public void JoinLobby()
        {
            PhotonNetwork.JoinLobby();
        }

        public void CreateRoom(string roomName, int maxPlayers)
        {
            if (!IsConnected) return;
            
            PhotonNetwork.CreateRoom(null, CreateRoomOptions((byte)maxPlayers));
        }

        public void JoinRoom()
        {
            if (!IsConnected) return;

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
            Debug.Log("Client connected to the server.");

            JoinLobby();
        }
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Client disconnected from the server.");
            
            _isConnected = false;
            _inRoom = false;
            SEventBus.ServerDisconnected?.Invoke();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log($"Client joined the lobby.");
            
            _isConnected = true;
            SEventBus.ServerConnected?.Invoke();
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
            Debug.Log("Left the room");

            _inRoom = false;
            SEventBus.RoomLeft?.Invoke();
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"Player named {newPlayer.NickName} entered the room.");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"Player named {otherPlayer.NickName} left the room.");
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log(
                $"Master client left the room. New master client name is {newMasterClient.NickName}"
            );
            
            SEventBus.MasterClientSwitched?.Invoke(newMasterClient);
        }
        
#endregion
    }
}
