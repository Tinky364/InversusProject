using UnityEngine.Events;

using Inversus.Helper;
using Inversus.Attribute;
using Inversus.Game;

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
        public UnityEvent RoundEnded;
        [ReadOnly]
        public UnityEvent<Player> PlayerHit;
        [ReadOnly]
        public UnityEvent GameEnded;

        protected override void Awake()
        {
            base.Awake();

            LoadSceneStarted = new UnityEvent();
            LoadSceneEnded = new UnityEvent();
            GameCreated = new UnityEvent();
            RoundStarted = new UnityEvent();
            RoundEnded = new UnityEvent();
            GameEnded = new UnityEvent();
            PlayerHit = new UnityEvent<Player>();
        }
    }
}
