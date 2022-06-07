using System.Collections;
using UnityEngine;

using Inversus.Manager;

using static Inversus.Facade;

namespace Inversus.UI.GameScene
{
    public class PausePanel : Panel
    {
        [SerializeField]
        private GameObject[] _children;

        private WaitForSeconds _wfs_1;

        private void Awake()
        {
            _wfs_1 = new WaitForSeconds(1);
            
            SEventBus.GamePaused.AddListener(OnGamePaused);
        }

        private void OnDestroy()
        {
            SEventBus.GamePaused.RemoveListener(OnGamePaused);
        }

        private void OnGamePaused(InputProfile inputProfile)
        {
            SCanvasManager.SetUiInputModule(inputProfile.PlayerInput);
            SetDisplay(true);
            SGameCanvasManager.BackgroundPanel.SetDisplay(true);
        }

        private void SetDisplayChildren(bool value)
        {
            for (int i = 0; i < _children.Length; i++)
                _children[i].SetActive(value);
        }

        public void ResumeButton_Click()
        {
            StartCoroutine(ResumeButton_ClickCor());
        }

        private IEnumerator ResumeButton_ClickCor()
        {
            SetDisplayChildren(false);
            Debug.Log("GameResumed Event => Invoke()");
            SEventBus.GameResumed?.Invoke();
            yield return _wfs_1;
            SGameCanvasManager.BackgroundPanel.SetDisplay(false);
            SetDisplayChildren(true);
            SetDisplay(false);
        }

        public void MainMenuButton_Click()
        {
            switch (SGameCreator.GameType)
            {
                case GameType.Local: 
                    SSceneCreator.LoadScene("MainMenuScene", SubSceneLoadMode.Single);
                    break;
                case GameType.Online:
                    SCanvasManager.SetUiInput(false);
                    SEventBus.LeaveRoomRequested?.Invoke();
                    break;
            }
        }
    }
}
