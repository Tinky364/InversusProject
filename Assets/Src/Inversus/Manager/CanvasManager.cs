using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Inversus.Helper;
using Inversus.Manager.Data;

using static Inversus.Manager.ManagerFacade;

namespace Inversus.Manager
{
    public class CanvasManager : SingletonMonoBehaviour<CanvasManager>
    {
        [SerializeField]
        private EventSystem _eventSystem;
        
        private GraphicRaycaster _graphicRaycaster;

        public EventSystem EventSystem => _eventSystem;
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;

        protected override void Awake()
        {
            base.Awake();

            _graphicRaycaster = GetComponent<GraphicRaycaster>();
        }

        public void ActivateUiInput(bool value)
        {
            _graphicRaycaster.enabled = value;
        }

        public void SetSelectedGameObject(GameObject element)
        {
            EventSystem.SetSelectedGameObject(element);
        }
        
        public void LoadScene(SceneData targetSceneData)
        {
            SSceneCreator.LoadScene(targetSceneData, SubSceneLoadMode.Single);
        }
    }
}
