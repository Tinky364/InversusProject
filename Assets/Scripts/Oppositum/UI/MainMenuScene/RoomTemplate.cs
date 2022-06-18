using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static Oppositum.Facade;

namespace Oppositum.UI.MainMenuScene
{
    public class RoomTemplate : MonoBehaviour
    {
        private Button _button;
        private TextMeshProUGUI _text;
        private EventTrigger _eventTrigger;

        private RoomListPanel _roomListPanel;
        private RoomInfo _roomInfo;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _eventTrigger = GetComponent<EventTrigger>();

            EventTrigger.Entry entry = new() {eventID = EventTriggerType.Select};
            entry.callback.AddListener(_ => SCanvasManager.PlayButtonSelectAudio());
            _eventTrigger.triggers.Add(entry);
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(Button_Click);
            _button.onClick.AddListener(SCanvasManager.PlayButtonClickAudio);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Button_Click);
            _button.onClick.RemoveListener(SCanvasManager.PlayButtonClickAudio);
        }

        private void Update()
        {
            if (_roomInfo.PlayerCount == _roomInfo.MaxPlayers)
            {
                _button.interactable = false;
                if (SMainMenuCanvasManager.LastSelectedGameObject == gameObject)
                {
                    SMainMenuCanvasManager.SetSelectedGameObject(
                        _roomListPanel.BackButton.gameObject
                    );
                }
            }
            else
            {
                _button.interactable = true;
            }
        }

        public void Initialize(RoomInfo roomInfo, RoomListPanel roomListPanel)
        {
            _roomInfo = roomInfo;
            _text.SetText(
                $"Room: {_roomInfo.Name}, {_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers}"
            );
            _roomListPanel = roomListPanel;
        }

        private void Button_Click()
        {
            _roomListPanel.JoinRoom_Click(_roomInfo);
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(_roomListPanel.SelectedRoom, _roomInfo))
            {
                _roomListPanel.OnSelectedRoomDestroyed();
            }
        }
    }
}
