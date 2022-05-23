using System.Collections;
using UnityEngine;

using Inversus.Manager;
using Inversus.UI;

using static Inversus.Facade;

namespace Inversus.Game
{
    public class GameCanvasManager : CanvasManager
    {
        [Header("PANELS")]
        [SerializeField]
        private Panel _backgroundPanel;
        [SerializeField]
        private Panel _pausePanel;
        [SerializeField]
        private RoundEndPanel _roundEndPanel;
        [SerializeField]
        private GameEndPanel _gameEndPanel;
        
        private WaitForSecondsRealtime _waitForSecondsRealtime1;
        
        protected override void Awake()
        {
            base.Awake();
            
            _waitForSecondsRealtime1 = new WaitForSecondsRealtime(1);
            
            _pausePanel.SetDisplay(false);
            _backgroundPanel.SetDisplay(false);
            _roundEndPanel.SetDisplay(false);
            _gameEndPanel.SetDisplay(false);
            
            SEventBus.GamePaused.AddListener(OnGamePaused);
            SEventBus.RoundEnded.AddListener(OnRoundEnded);
            SEventBus.RoundStartRequested.AddListener(OnRoundStartRequested);
            SEventBus.GameEnded.AddListener(OnGameEnded);
        }

        private void OnGamePaused(Player player)
        {
            InputSystemUIInputModule.actionsAsset = player.PlayerInput.actions;
            _pausePanel.SetDisplay(true);
            _backgroundPanel.SetDisplay(true);
        }

        public void OnPauseResumeButtonClick()
        {
            StartCoroutine(OnPauseResumeButtonClickCor());
        }

        private IEnumerator OnPauseResumeButtonClickCor()
        {
            _pausePanel.SetDisplay(false);
            Debug.Log("GameResumed Event => Invoke()");
            SEventBus.GameResumed?.Invoke();
            yield return _waitForSecondsRealtime1;
            _backgroundPanel.SetDisplay(false);
        }

        private void OnRoundEnded(int player1Score, int player2Score, string roundWinnerName)
        {
            _roundEndPanel.SetDisplay(true);
            _roundEndPanel.SetRoundEndPanel(
                SGameCreator.Round, SGameCreator.VictoryScore, player1Score, player2Score, 
                SGameCreator.Player1.Name, SGameCreator.Player2.Name, roundWinnerName
            );
            StartCoroutine(OnRoundEndedCor());
        }

        private IEnumerator OnRoundEndedCor()
        {
            _roundEndPanel.SetCountText("5");
            yield return _waitForSecondsRealtime1;
            _roundEndPanel.SetCountText("4");
            yield return _waitForSecondsRealtime1;
            _roundEndPanel.SetCountText("3");
            yield return _waitForSecondsRealtime1;
            _roundEndPanel.SetCountText("2");
            yield return _waitForSecondsRealtime1;
            _roundEndPanel.SetCountText("1");
            yield return _waitForSecondsRealtime1;
        }

        private void OnRoundStartRequested()
        {
            _roundEndPanel.SetDisplay(false);
        }

        private void OnGameEnded(int player1Score, int player2Score, string winnerName)
        {
            _gameEndPanel.SetDisplay(true);
            _gameEndPanel.SetGameEndPanel(
                player1Score, player2Score, SGameCreator.Player1.Name, SGameCreator.Player2.Name,
                winnerName
            );
        }
        
        public void OnGameEndRetryButtonClick()
        {
            StartCoroutine(OnGameEndRetryButtonClickCor());
        }
        
        private IEnumerator OnGameEndRetryButtonClickCor()
        {
            _gameEndPanel.SetDisplay(false);
            _backgroundPanel.SetDisplay(true);
            yield return _waitForSecondsRealtime1;
            Debug.Log("GameEndRetryButtonClicked Event => Invoke()");
            SEventBus.RetryLocalGameRequested?.Invoke();
            _backgroundPanel.SetDisplay(false);
        }
        
        private void OnDestroy()
        {
            SEventBus.GamePaused.RemoveListener(OnGamePaused);
            SEventBus.RoundEnded.RemoveListener(OnRoundEnded);
            SEventBus.RoundStartRequested.RemoveListener(OnRoundStartRequested);
            SEventBus.GameEnded.RemoveListener(OnGameEnded);
        }
    }
}

