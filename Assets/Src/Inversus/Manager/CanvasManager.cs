using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

using Inversus.Helper;
using Inversus.Data;
using Inversus.UI;
using UnityEngine.InputSystem;
using static Inversus.Facade;

namespace Inversus.Manager
{
    public class CanvasManager : SingletonMonoBehaviour<CanvasManager>
    {
        [SerializeField]
        private EventSystem _eventSystem;
        public EventSystem EventSystem => _eventSystem;

        [SerializeField]
        protected Panel _foregroundPanel;

        public InputSystemUIInputModule InputSystemUIInputModule { get; private set; }

        private GameObject _lastSelectedGameObject;
        
        protected override void Awake()
        {
            base.Awake();
            
            InputSystemUIInputModule = _eventSystem.GetComponent<InputSystemUIInputModule>();
        }

        public void SetUiInputModule(PlayerInput playerInput)
        {
            playerInput.uiInputModule = InputSystemUIInputModule;
        }

        public void SetSelectedGameObject(GameObject element)
        {
            if (EventSystem == null) return;
            EventSystem.SetSelectedGameObject(element);
        }
        
        public void LoadScene(SceneData targetSceneData)
        {
            SSceneCreator.LoadScene(targetSceneData, SubSceneLoadMode.Single);
        }
        
        public void SetUiInput(bool isActive)
        {
            if (isActive)
            {
               EventSystem.SetSelectedGameObject(_lastSelectedGameObject);
                _foregroundPanel.SetDisplay(false);
            }
            else
            {
                _lastSelectedGameObject = EventSystem.currentSelectedGameObject;
                EventSystem.SetSelectedGameObject(null);
                _foregroundPanel.SetDisplay(true);
            }
        }
    }
}
