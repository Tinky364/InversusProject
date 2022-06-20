using System.Collections;
using UnityEngine;
using Oppositum.Attribute;
using Oppositum.Data;
using Oppositum.Manager;
using Photon.Realtime;
using static Oppositum.Facade;

namespace Oppositum.Game
{
    public class GameSubSceneManager : SubSceneManager
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private BulletPool _bulletPool;
        [SerializeField, Expandable]
        private AudioData _gameAudioData;
        
        public BulletPool BulletPool => _bulletPool;

        private AudioSource _audioSource;
        private WaitForSeconds _wfs_1;
        private WaitForSeconds _wfs_3;
        private WaitForSeconds _wfs_2_6;

        protected override void Awake()
        {
            base.Awake();

            _audioSource = GetComponent<AudioSource>();
            _wfs_1 = new WaitForSeconds(1f);
            _wfs_3 = new WaitForSeconds(3f);
            _wfs_2_6 = new WaitForSeconds(2.6f);
            
            SEventBus.GamePaused.AddListener(OnGamePaused);
            SEventBus.GameResumed.AddListener(OnGameResumed);
            SEventBus.RoundEnded.AddListener(OnRoundEnded);
            SEventBus.RoundCreated.AddListener(OnRoundCreated);
            SEventBus.PlayerLeftRoom.AddListener(OnPlayerLeftRoom);
            SEventBus.RoomLeft.AddListener(OnRoomLeft);
        }

        protected override void OnSceneLoaded(SceneData sceneData)
        {
            _camera.backgroundColor = SGameCreator.ColorTheme.BackgroundColor;
            SGameCreator.CreateGame();
            _gameAudioData.Play(_audioSource);
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
            yield return _wfs_2_6;
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

        private void OnPlayerLeftRoom(Player otherPlayer)
        {
            SEventBus.LeaveRoomRequested?.Invoke();
        }

        private void OnRoomLeft()
        {
            SSceneCreator.LoadScene("MainMenuScene", SubSceneLoadMode.Single);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            SEventBus.GamePaused.RemoveListener(OnGamePaused);
            SEventBus.GameResumed.RemoveListener(OnGameResumed);
            SEventBus.RoundEnded.RemoveListener(OnRoundEnded);
            SEventBus.RoundCreated.RemoveListener(OnRoundCreated);
            SEventBus.PlayerLeftRoom.RemoveListener(OnPlayerLeftRoom);
            SEventBus.RoomLeft.RemoveListener(OnRoomLeft);
        }
    }
}
