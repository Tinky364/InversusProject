using System.Collections;
using UnityEngine;

using static Inversus.Facade;

namespace Inversus.UI
{
    public class PlayOnlinePanel : Panel
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
            
            SetDisplay(false);
            SMainMenuCanvasManager.PlayPanel.SetDisplay(true);
        }
        
#region Create Room
        public void CreateRoomButton_Click()
        {
            StartCoroutine(CreateRoomButton_ClickCor());
        }

        private IEnumerator CreateRoomButton_ClickCor()
        {
            SCanvasManager.SetUiInput(false);
            SEventBus.CreateRoomRequested?.Invoke("Room1", 2);
            float duration = 0f;
            while (duration < 5f)
            {
                if (SOnlineGameManager.InRoom)
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
#endregion

#region Join Room
        public void JoinRoomButton_Click()
        {
            SMainMenuCanvasManager.RoomListPanel.SetDisplay(true);
            SetDisplay(false);
        }
#endregion
    }
}
