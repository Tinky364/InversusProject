using UnityEngine;

using Inversus.Helper;
using Inversus.Manager.Data;

using static Inversus.Manager.ManagerFacade;

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

        protected virtual void OnSceneLoaded()
        {
            
        }

        public void LoadScene(SceneData targetSceneData)
        {
            SSubSceneCreator.LoadScene(targetSceneData, SubSceneLoadMode.Single);
        }
    }
}
