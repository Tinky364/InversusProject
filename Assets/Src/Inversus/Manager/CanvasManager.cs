using UnityEngine;
using UnityEngine.UI;

using Inversus.Helper;

namespace Inversus.Manager
{
    public class CanvasManager : SingletonMonoBehaviour<CanvasManager>
    {
        [SerializeField] private GraphicRaycaster _graphicRaycaster = default;

        public void ActivateUiInput(bool value)
        {
            _graphicRaycaster.enabled = value;
        }
    }
}
