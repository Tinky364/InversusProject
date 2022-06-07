using UnityEngine.Events;
using Photon.Realtime;

using Inversus.Helper;
using Inversus.Attribute;
using Inversus.Data;
using Inversus.Game;

namespace Inversus.Manager
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
        public UnityEvent<int, int, string> RoundEnded;
        [ReadOnly]
        public UnityEvent<int, int, string> GameEnded;
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
        public UnityEvent ServerConnected;
        [ReadOnly]
        public UnityEvent ServerDisconnected;
        
        [ReadOnly]
        public UnityEvent<string, int> CreateRoomRequested;
        [ReadOnly]
        public UnityEvent<string> RoomCreated;
        [ReadOnly]
        public UnityEvent RoomCreateFailed;

        [ReadOnly]
        public UnityEvent JoinRoomRequested;
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
