using UnityEngine;
using Oppositum.Data;
using Oppositum.Helper;
using static Oppositum.Facade;

namespace Oppositum.Manager
{
    public class SubSceneManager : SingletonMonoBehaviour<SubSceneManager>
    {
        [SerializeField]
        private SceneData _sceneData;
        public SceneData SceneData => _sceneData;

        protected override void Awake()
        {
            base.Awake();
            
            SEventBus.LoadSceneEnded.AddListener(OnSceneLoaded);
        }

        protected virtual void OnDestroy()
        {
            SEventBus.LoadSceneEnded.RemoveListener(OnSceneLoaded);
        }

        protected virtual void OnSceneLoaded(SceneData sceneData)
        {
            
        }
    }
}
