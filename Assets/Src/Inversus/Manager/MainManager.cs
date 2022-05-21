using UnityEngine;

using Inversus.Helper;
using Inversus.Attribute;
using Inversus.Data;

using static Inversus.Facade;

namespace Inversus.Manager
{
    public class MainManager : SingletonMonoBehaviour<MainManager>
    {
        [SerializeField, ReadOnly] 
        private SubSceneManager _subManager;
        [SerializeField] 
        private bool _editorMode;
        [SerializeField] 
        private SceneData _startingSceneData;

        private States _state;
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
            SSceneCreator.LoadScene(_startingSceneData, SubSceneLoadMode.Single);
#endif

            Debug.Log("Manager Scene => Start()");
        }

        private void StartInEditor()
        {
            if (EditorMode)
            {
                if (SSceneCreator.LoadedSceneCount == 1)
                {
                    SSceneCreator.LoadScene(_startingSceneData, SubSceneLoadMode.Single);
                }
                else if (SSceneCreator.LoadedSceneCount == 2)
                {
                    SSceneCreator.SetActiveScene(SSubSceneManager.SceneData); // Because LoadScene method will unload active scene.
                    SSceneCreator.LoadScene(SSubSceneManager.SceneData, SubSceneLoadMode.Single);
                }
                else
                {
                    SSceneCreator.LoadManagerScene();
                }
            }
            else
            {
                if (SSceneCreator.LoadedSceneCount == 1)
                {
                    SSceneCreator.LoadScene(_startingSceneData, SubSceneLoadMode.Single);
                }
                else
                {
                    SSceneCreator.LoadManagerScene();
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
            SSceneCreator.SetActiveScene(_subManager.SceneData);
            return true;
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
