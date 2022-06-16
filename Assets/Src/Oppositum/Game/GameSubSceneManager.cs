using System.Collections;
using UnityEngine;
using Oppositum.Data;
using Oppositum.Manager;
using static Oppositum.Facade;

namespace Oppositum.Game
{
    public class GameSubSceneManager : SubSceneManager
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private BulletPool _bulletPool;
        
        public BulletPool BulletPool => _bulletPool;

        private WaitForSeconds _wfs_1;
        private WaitForSeconds _wfs_3;
        private WaitForSeconds _wfs_2_1;

        protected override void Awake()
        {
            base.Awake();
            
            _wfs_1 = new WaitForSeconds(1);
            _wfs_3 = new WaitForSeconds(3);
            _wfs_2_1 = new WaitForSeconds(2.1f);
            
            SEventBus.GamePaused.AddListener(OnGamePaused);
            SEventBus.GameResumed.AddListener(OnGameResumed);
            SEventBus.RoundEnded.AddListener(OnRoundEnded);
            SEventBus.RoundCreated.AddListener(OnRoundCreated);
        }

        protected override void OnSceneLoaded(SceneData sceneData)
        {
            _camera.backgroundColor = SGameCreator.ColorTheme.BackgroundColor;
            SGameCreator.CreateGame();
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
            yield return _wfs_1;
            SMainManager.State = States.InGame;
        }

        private void OnRoundCreated()
        {
            StartCoroutine(OnRoundCreatedCor());
        }

        private IEnumerator OnRoundCreatedCor()
        {
            yield return _wfs_2_1;
            Debug.Log("RoundStarted Event => Invoke()");
            SEventBus.RoundStarted?.Invoke();
            SMainManager.State = States.InGame;
        }

        private void OnRoundEnded(
            PlayerController player1, PlayerController player2, PlayerController winner
        )
        {
            StartCoroutine(OnRoundEndedCor());
        }

        private IEnumerator OnRoundEndedCor()
        {
            yield return _wfs_3;
            Debug.Log("RoundStartRequested Event => Invoke()");
            SEventBus.RoundStartRequested?.Invoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            SEventBus.GamePaused.RemoveListener(OnGamePaused);
            SEventBus.GameResumed.RemoveListener(OnGameResumed);
            SEventBus.RoundEnded.RemoveListener(OnRoundEnded);
            SEventBus.RoundCreated.RemoveListener(OnRoundCreated);
        }
    }
}
