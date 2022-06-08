using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

using Inversus.Helper;
using Inversus.Data;
using Inversus.UI;

using static Inversus.Facade;

namespace Inversus.Manager
{
    public class CanvasManager : SingletonMonoBehaviour<CanvasManager>
    {
        [SerializeField]
        private EventSystem _eventSystem;
        [SerializeField]
        private Panel _backgroundPanel;
        [SerializeField]
        private Panel _foregroundPanel;

        public Panel BackgroundPanel => _backgroundPanel;
        public Panel ForegroundPanel => _foregroundPanel;
        public GameObject LastSelectedGameObject => _lastSelectedGameObject;

        private InputSystemUIInputModule _inputSystemUIInputModule;
        private GameObject _lastSelectedGameObject;
        
        protected override void Awake()
        {
            base.Awake();
            
            _inputSystemUIInputModule = _eventSystem.GetComponent<InputSystemUIInputModule>();
        }

        public void SetUiInputModule(PlayerInput playerInput)
        {
            playerInput.uiInputModule = _inputSystemUIInputModule;
        }

        public void SetSelectedGameObject(GameObject element)
        {
            if (_eventSystem == null) return;
            _eventSystem.SetSelectedGameObject(element);
        }
        
        public void LoadScene(SceneData targetSceneData)
        {
            SSceneCreator.LoadScene(targetSceneData, SubSceneLoadMode.Single);
        }
        
        public void SetUiInput(bool isActive)
        {
            if (isActive)
            {
               _eventSystem.SetSelectedGameObject(_lastSelectedGameObject);
                _foregroundPanel.SetDisplay(false);
            }
            else
            {
                _lastSelectedGameObject = _eventSystem.currentSelectedGameObject;
                _eventSystem.SetSelectedGameObject(null);
                _foregroundPanel.SetDisplay(true);
            }
        }
    }
}
