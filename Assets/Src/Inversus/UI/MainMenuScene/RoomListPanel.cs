using System.Collections;
using UnityEngine;

using static Inversus.Facade;

namespace Inversus.UI.MainMenuScene
{
    public class RoomListPanel : Panel
    {
        private void OnEnable()
        {
            SEventBus.ServerDisconnected.AddListener(OnServerDisconnected);
        }

        private void OnDisable()
        {
            SEventBus.ServerDisconnected.RemoveListener(OnServerDisconnected);
        }

        private void OnServerDisconnected()
        {
            if (SMainManager.State != States.PlayOnlineMenu) return;
            
            SMainMenuCanvasManager.PlayPanel.SetDisplay(true);
            SetDisplay(false);
        }
        
        public void JoinRoomButton_Click()
        {
            StartCoroutine(JoinRoomButton_ClickCor());
        }
        
        private IEnumerator JoinRoomButton_ClickCor()
        {
            SCanvasManager.SetUiInput(false);
            SEventBus.JoinRoomRequested?.Invoke();
            float duration = 0f;
            while (duration < 5f)
            {
                if (SOnlineManager.InRoom)
                {
                    SCanvasManager.SetUiInput(true);
                    SMainMenuCanvasManager.RoomPanel.SetDisplay(true);
                    SetDisplay(false);
                    yield break;
                }
                duration += Time.deltaTime;
                yield return null;
            }
            SCanvasManager.SetUiInput(true);
        }
    }
}
