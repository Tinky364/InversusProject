using UnityEngine.Events;

using Inversus.Helper;
using Inversus.Attribute;

namespace Inversus.Manager
{
    public class EventBus : SingletonMonoBehaviour<EventBus>
    {
        [ReadOnly] 
        public UnityEvent LoadSceneStarted;
        [ReadOnly] 
        public UnityEvent LoadSceneEnded;
        [ReadOnly]
        public UnityEvent GameCreated; 
        [ReadOnly]
        public UnityEvent RoundStarted;
        [ReadOnly]
        public UnityEvent RoundStartRequested;
        [ReadOnly]
        public UnityEvent<int, int, string> RoundEnded;
        [ReadOnly]
        public UnityEvent<Player> PlayerHit;
        [ReadOnly]
        public UnityEvent<int, int, string> GameEnded;
        [ReadOnly]
        public UnityEvent<Player> PlayerJoinedGame;
        [ReadOnly]
        public UnityEvent<Player> PlayerLeftGame;
        [ReadOnly]
        public UnityEvent<Player> GamePaused;
        [ReadOnly]
        public UnityEvent GameResumed;
        [ReadOnly]
        public UnityEvent<int, int, int> PlayLocallyStartGameButtonClicked;
        [ReadOnly]
        public UnityEvent GameEndRetryButtonClicked;

        protected override void Awake()
        {
            base.Awake();

            LoadSceneStarted = new UnityEvent();
            LoadSceneEnded = new UnityEvent();
            GameCreated = new UnityEvent();
            RoundStarted = new UnityEvent();
            RoundStartRequested = new UnityEvent();
            RoundEnded = new UnityEvent<int, int, string>();
            GameEnded = new UnityEvent<int, int, string>();
            PlayerHit = new UnityEvent<Player>();
            PlayerJoinedGame = new UnityEvent<Player>();
            PlayerLeftGame = new UnityEvent<Player>();
            GamePaused = new UnityEvent<Player>();
            GameResumed = new UnityEvent();
            PlayLocallyStartGameButtonClicked = new UnityEvent<int, int, int>();
            GameEndRetryButtonClicked = new UnityEvent();
        }
    }
}
