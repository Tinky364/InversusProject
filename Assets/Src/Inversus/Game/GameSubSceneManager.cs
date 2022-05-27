using System.Collections;
using UnityEngine;

using Inversus.Data;
using Inversus.Manager;

using static Inversus.Facade;

namespace Inversus.Game
{
    public class GameSubSceneManager : SubSceneManager
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private BulletPool _bulletPool;
        
        public BulletPool BulletPool => _bulletPool;

        private WaitForSeconds _waitForSeconds1;
        private WaitForSeconds _waitForSeconds5;

        protected override void Awake()
        {
            base.Awake();
            
            _waitForSeconds1 = new WaitForSeconds(1);
            _waitForSeconds5 = new WaitForSeconds(5);
            
            SEventBus.GamePaused.AddListener(OnGamePaused);
            SEventBus.GameResumed.AddListener(OnGameResumed);
            SEventBus.RoundEnded.AddListener(OnRoundEnded);
        }

        protected override void OnSceneLoaded(SceneData sceneData)
        {
            _camera.backgroundColor = SGameCreator.ColorTheme.BackgroundColor;
            SGameCreator.CreateGame();
            _bulletPool.Initialize();
        }

        private void OnGamePaused(InputProfile inputProfile)
        {
            SGameCreator.PlayerControllers[0].Pause();
            SGameCreator.PlayerControllers[1].Pause();
            SMainManager.State = States.GamePauseMenu;
        }

        private void OnGameResumed()
        {
            StartCoroutine(OnGameResumedCor());
        }

        private IEnumerator OnGameResumedCor()
        {
            yield return _waitForSeconds1;
            SMainManager.State = States.InGame;
        }
        
        private void OnRoundEnded(int player1Score, int player2Score, string roundWinnerName)
        {
            StartCoroutine(OnRoundEndedCor());
        }

        private IEnumerator OnRoundEndedCor()
        {
            yield return _waitForSeconds5;
            Debug.Log("RoundStartRequested Event => Invoke()");
            SEventBus.RoundStartRequested?.Invoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            SEventBus.GamePaused.RemoveListener(OnGamePaused);
            SEventBus.GameResumed.RemoveListener(OnGameResumed);
            SEventBus.RoundEnded.RemoveListener(OnRoundEnded);
        }
    }
}
