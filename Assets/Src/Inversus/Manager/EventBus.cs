using UnityEngine.Events;

using Inversus.Helper;
using Inversus.Attribute;

namespace Inversus.Manager
{
    public class EventBus : SingletonMonoBehaviour<EventBus>
    {
        [ReadOnly] public UnityEvent LoadSceneStarted;
        [ReadOnly] public UnityEvent LoadSceneEnded;
        
        protected override void Awake()
        {
            base.Awake();

            LoadSceneStarted = new UnityEvent();
            LoadSceneEnded = new UnityEvent();
        }
    }
}
