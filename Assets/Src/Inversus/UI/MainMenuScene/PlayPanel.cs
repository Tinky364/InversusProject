using System.Collections;
using UnityEngine;

using static Inversus.Facade;

namespace Inversus.UI
{
    public class PlayPanel : Panel
    {
        public void PlayOnlineButton_Click()
        {
            StartCoroutine(PlayOnlineButton_ClickCor());
        }

        private IEnumerator PlayOnlineButton_ClickCor()
        {
            SCanvasManager.SetUiInput(false);
            SEventBus.ServerConnectionRequested?.Invoke();
            float duration = 0;
            while (duration < 10f)
            {
                if (SOnlineGameManager.IsConnected)
                {
                    SCanvasManager.SetUiInput(true);
                    SMainMenuCanvasManager.PlayOnlinePanel.SetDisplay(true);
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
