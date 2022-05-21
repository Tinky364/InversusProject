using System.Collections;
using UnityEngine;

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

        private WaitForSecondsRealtime _waitForSecondsRealtime1;
        private WaitForSecondsRealtime _waitForSecondsRealtime5;

        protected override void Awake()
        {
            base.Awake();
            
            _waitForSecondsRealtime1 = new WaitForSecondsRealtime(1);
            _waitForSecondsRealtime5 = new WaitForSecondsRealtime(5);
            
            SEventBus.GamePaused.AddListener(OnGamePaused);
            SEventBus.GameResumed.AddListener(OnGameResumed);
            SEventBus.RoundEnded.AddListener(OnRoundEnded);
        }

        protected override void OnSceneLoaded()
        {
            _bulletPool.Initialize();
            SGameCreator.CreateGame();
            _camera.backgroundColor = SGameCreator.ColorTheme.BackgroundColor;
        }

        private void OnGamePaused(Player player)
        {
            SMainManager.State = States.GamePauseMenu;
            Time.timeScale = 0;
        }

        private void OnGameResumed()
        {
            StartCoroutine(OnGameResumedCor());
        }

        private IEnumerator OnGameResumedCor()
        {
            yield return _waitForSecondsRealtime1;
            SMainManager.State = States.InGame;
            Time.timeScale = 1;
        }
        
        private void OnRoundEnded(int player1Score, int player2Score, string roundWinnerName)
        {
            StartCoroutine(OnRoundEndedCor());
        }

        private IEnumerator OnRoundEndedCor()
        {
            yield return _waitForSecondsRealtime5;
            Debug.Log("RoundStartRequested Event => Invoke()");
            SEventBus.RoundStartRequested?.Invoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            SEventBus.GamePaused.RemoveListener(OnGamePaused);
            SEventBus.GameResumed.RemoveListener(OnGameResumed);
            SEventBus.RoundEnded.RemoveListener(OnRoundEnded);
            
            Time.timeScale = 1;
        }
    }
}
