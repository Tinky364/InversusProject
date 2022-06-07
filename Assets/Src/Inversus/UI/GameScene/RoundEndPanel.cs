using System.Collections;
using UnityEngine;
using TMPro;

using static Inversus.Facade;

namespace Inversus.UI.GameScene
{
    public class RoundEndPanel : Panel
    {
        [SerializeField]
        private TextMeshProUGUI _roundText;
        [SerializeField]
        private TextMeshProUGUI _winnerText;
        [SerializeField]
        private TextMeshProUGUI _player1Text;
        [SerializeField]
        private TextMeshProUGUI _player2Text;
        [SerializeField]
        private TextMeshProUGUI _countText;
        
        private WaitForSeconds _wfs_1;

        private void Awake()
        {
            _wfs_1 = new WaitForSeconds(1);
            
            SEventBus.RoundEnded.AddListener(OnRoundEnded);
            SEventBus.RoundStartRequested.AddListener(OnRoundStartRequested);
        }

        private void OnDestroy()
        {
            SEventBus.RoundEnded.RemoveListener(OnRoundEnded);
            SEventBus.RoundStartRequested.RemoveListener(OnRoundStartRequested);
        }
        
        private void OnRoundEnded(int player1Score, int player2Score, string roundWinnerName)
        {
            SetDisplay(true);
            SetRoundEndPanel(
                SGameCreator.Round, SGameCreator.VictoryScore, player1Score, player2Score,
                SGameCreator.PlayerControllers[0].PlayerName,
                SGameCreator.PlayerControllers[1].PlayerName,
                roundWinnerName
            );
            StartCoroutine(OnRoundEndedCor());
        }

        private IEnumerator OnRoundEndedCor()
        {
            SetCountText("5");
            yield return _wfs_1;
            SetCountText("4");
            yield return _wfs_1;
            SetCountText("3");
            yield return _wfs_1;
            SetCountText("2");
            yield return _wfs_1;
            SetCountText("1");
            yield return _wfs_1;
        }

        private void OnRoundStartRequested()
        {
            SetDisplay(false);
        }
        
        private void SetRoundEndPanel(
            int currentRound, int victoryScore, int player1Score, int player2Score, 
            string player1Name, string player2Name, string roundWinnerName
        )
        {
            _roundText.SetText($"ROUND: {currentRound} - VICTORY SCORE: {victoryScore}");
            _winnerText.SetText($"Round Winner: {roundWinnerName}");
            _player1Text.SetText($"{player1Name}<br>Score<br>{player1Score}");
            _player2Text.SetText($"{player2Name}<br>Score<br>{player2Score}");
        }

        private void SetCountText(string count)
        {
            _countText.SetText(count);
        }
    }
}
