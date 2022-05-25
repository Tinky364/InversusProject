using UnityEngine;

using Inversus.Helper;
using Inversus.Data;

using static Inversus.Facade;

namespace Inversus.Manager
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
