using System.Collections;
using UnityEngine;

using static Oppositum.Facade;

namespace Oppositum.UI.MainMenuScene
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
            
            SEventBus.JoinLobbyRequested?.Invoke();
            float duration = 0f;
            while (duration < 5f)
            {
                if (SOnlineManager.InLobby) break;
                duration += Time.deltaTime;
                yield return null;
            }

            if (SOnlineManager.InLobby)
            {
                SEventBus.CreateRoomRequested?.Invoke("Room1", 2);
                duration = 0f;
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
            }
            
            SCanvasManager.SetUiInput(true);
        }
#endregion

#region Join Room
        public void JoinRoomButton_Click()
        {
            StartCoroutine(JoinRoomButton_ClickCor());
        }

        private IEnumerator JoinRoomButton_ClickCor()
        {
            SCanvasManager.SetUiInput(false);

            SEventBus.JoinLobbyRequested?.Invoke();
            float duration = 0f;
            while (duration < 5f)
            {
                if (SOnlineManager.InLobby)
                {
                    SCanvasManager.SetUiInput(true);
                    SMainMenuCanvasManager.RoomListPanel.SetDisplay(true);
                    SetDisplay(false);
                    yield break;
                }
                duration += Time.deltaTime;
                yield return null;
            }
            
            SCanvasManager.SetUiInput(true);
        }
#endregion
    }
}
