using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static Inversus.Manager.ManagerFacade;

namespace Inversus.UI
{
    public class ButtonX : Selectable
    {
        [SerializeField] private bool _hasText = false;
        [SerializeField] private TextMeshProUGUI _textMesh = default;
        
        public UnityEvent ButtonExecuted;
        public bool HasText => _hasText && _textMesh;

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable()) return;

            base.OnPointerDown(eventData);
            SCanvasManager.ActivateUiInput(false);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable()) return;

            base.OnPointerUp(eventData);
            ButtonExecuted?.Invoke();
            SCanvasManager.ActivateUiInput(true);
        }
    }
}
