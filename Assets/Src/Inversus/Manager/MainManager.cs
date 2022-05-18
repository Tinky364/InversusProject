using UnityEngine;

using Inversus.Helper;
using Inversus.Attribute;
using Inversus.Manager.Data;

using static Inversus.Manager.ManagerFacade;

namespace Inversus.Manager
{
    public enum States { MainMenu, Loading, InGame, GamePauseMenu }
    
    public class MainManager : SingletonMonoBehaviour<MainManager>
    {
        [SerializeField, ReadOnly] 
        private SubSceneManager _subManager;
        [SerializeField] 
        private bool _editorMode;
        [SerializeField] 
        private SceneData _startingSceneData;

        public States _state;
        public States State { get => _state;
            set
            {
                if (value == _state) return;
                
                _state = value;
                Debug.Log($"STATE: {State}");
            } 
        }        

        public bool EditorMode => _editorMode;

        protected override void Awake()
        {
            base.Awake();

            State = States.Loading;

#if UNITY_EDITOR
            Debug.Log("Editor Mode: On");
#else
            if (EditorMode)
                _editorMode = false;
            Debug.Log("Editor Mode: Off");
#endif

            Debug.Log("Manager Scene => Awake()");
        }

        private void Start()
        {
#if UNITY_EDITOR
            StartInEditor();
#else
            SceneManager.Instance.LoadScene(_startingSceneData, LoadSceneMode.SINGLE);
#endif
            Debug.Log("Manager Scene => Start()");
        }

        private void StartInEditor()
        {
            if (EditorMode)
            {
                if (SSubSceneCreator.LoadedSceneCount == 1)
                {
                    SSubSceneCreator.LoadScene(_startingSceneData, SubSceneLoadMode.Single);
                }
                else if (SSubSceneCreator.LoadedSceneCount == 2)
                {
                    SSubSceneCreator.SetActiveScene(SSubSceneManager.SceneData); // Because LoadScene method will unload active scene.
                    SSubSceneCreator.LoadScene(SSubSceneManager.SceneData, SubSceneLoadMode.Single);
                }
                else
                {
                    SSubSceneCreator.LoadManagerScene();
                }
            }
            else
            {
                if (SSubSceneCreator.LoadedSceneCount == 1)
                {
                    SSubSceneCreator.LoadScene(_startingSceneData, SubSceneLoadMode.Single);
                }
                else
                {
                    SSubSceneCreator.LoadManagerScene();
                }
            }
        }

        public bool InitializeSubManager()
        {
            if (SSubSceneManager == null)
            {
                Debug.LogError("Initializing Sub Manager failed!");
                return false;
            }
            _subManager = SSubSceneManager;
            SSubSceneCreator.SetActiveScene(_subManager.SceneData);
            return true;
        }
    }
}
