using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

using Inversus.Helper;
using Inversus.Data;

using static Inversus.Facade;

namespace Inversus.Manager
{
    public class CanvasManager : SingletonMonoBehaviour<CanvasManager>
    {
        [SerializeField]
        private EventSystem _eventSystem;
       
        public EventSystem EventSystem => _eventSystem;
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;
        public InputSystemUIInputModule InputSystemUIInputModule => _inputSystemUIInputModule;

        private GraphicRaycaster _graphicRaycaster;
        private InputSystemUIInputModule _inputSystemUIInputModule;
        
        protected override void Awake()
        {
            base.Awake();

            _graphicRaycaster = GetComponent<GraphicRaycaster>();
            _inputSystemUIInputModule = _eventSystem.GetComponent<InputSystemUIInputModule>();
        }

        public void ActivateUiInput(bool value)
        {
            _graphicRaycaster.enabled = value;
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
    }
}
