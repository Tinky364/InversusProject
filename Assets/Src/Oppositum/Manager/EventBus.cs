using UnityEngine.Events;
using Photon.Realtime;
using Oppositum.Attribute;
using Oppositum.Data;
using Oppositum.Game;
using Oppositum.Helper;

namespace Oppositum.Manager
{
    public class EventBus : SingletonMonoBehaviour<EventBus>
    {
        [ReadOnly] 
        public UnityEvent LoadSceneStarted;
        [ReadOnly] 
        public UnityEvent<SceneData> LoadSceneEnded;
        
        [ReadOnly]
        public UnityEvent GameCreated; 
        [ReadOnly]
        public UnityEvent RoundStartRequested;
        [ReadOnly]
        public UnityEvent RoundCreated;
        [ReadOnly]
        public UnityEvent RoundStarted;
        [ReadOnly]
        public UnityEvent<PlayerController, PlayerController, PlayerController> RoundEnded;
        [ReadOnly]
        public UnityEvent<PlayerController, PlayerController, PlayerController> GameEnded;
        [ReadOnly]
        public UnityEvent<InputProfile> GamePaused;
        [ReadOnly]
        public UnityEvent GameResumed;
        [ReadOnly]
        public UnityEvent PlayAgainGameRequested;
        [ReadOnly]
        public UnityEvent<PlayerController> PlayerHit;
        
        [ReadOnly]
        public UnityEvent<InputProfile> InputProfileJoined;
        [ReadOnly]
        public UnityEvent<InputProfile> InputProfileLeft;
        [ReadOnly]
        public UnityEvent<int, int, int, GameType> StartGameRequested;

        [ReadOnly]
        public UnityEvent ServerConnectionRequested;
        [ReadOnly]
        public UnityEvent JoinLobbyRequested;
        [ReadOnly]
        public UnityEvent LeaveLobbyRequested;
        [ReadOnly]
        public UnityEvent ServerConnected;
        [ReadOnly]
        public UnityEvent ServerDisconnected;
        [ReadOnly]
        public UnityEvent LobbyJoined;
        [ReadOnly]
        public UnityEvent LobbyLeft;
        
        [ReadOnly]
        public UnityEvent<string, int> CreateRoomRequested;
        [ReadOnly]
        public UnityEvent<string> RoomCreated;
        [ReadOnly]
        public UnityEvent RoomCreateFailed;

        [ReadOnly]
        public UnityEvent<string> JoinRoomRequested;
        [ReadOnly]
        public UnityEvent RoomJoined;
        [ReadOnly]
        public UnityEvent RoomJoinFailed;
        
        [ReadOnly]
        public UnityEvent LeaveRoomRequested;
        [ReadOnly]
        public UnityEvent RoomLeft;
        [ReadOnly]
        public UnityEvent RoomLeaveFailed;
        [ReadOnly]
        public UnityEvent<Player> MasterClientSwitched;
        [ReadOnly]
        public UnityEvent<Player> PlayerLeftRoom;
    }
}
