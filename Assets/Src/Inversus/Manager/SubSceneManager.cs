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

            Debug.Log($"{nameof(SubSceneManager)} => Awake()");
        }

        protected virtual void Start()
        {
            Debug.Log($"{nameof(SubSceneManager)} => Start()");
        }

        protected virtual void OnEnable()
        {
            Debug.Log($"{nameof(SubSceneManager)} => OnEnable()");
        }

        protected virtual void OnDisable()
        {
            Debug.Log($"{nameof(SubSceneManager)} => OnDisable()");
        }

        public void LoadScene(SceneData targetSceneData)
        {
            SSubSceneCreator.LoadScene(targetSceneData, SubSceneLoadMode.Single);
        }
    }
}
