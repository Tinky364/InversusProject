using System.Collections;
using Inversus.Game;
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

        private void OnRoundEnded(
            PlayerController player1, PlayerController player2, PlayerController winner
        )
        {
            SetDisplay(true);
            SetRoundEndPanel(player1, player2, winner);
            _countText.color = winner.Side.PlayerColor;
            StartCoroutine(OnRoundEndedCor());
        }

        private IEnumerator OnRoundEndedCor()
        {
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
            PlayerController player1, PlayerController player2, PlayerController winner
        )
        {
            _roundText.SetText(
                $"ROUND: {SGameCreator.Round} - VICTORY SCORE: {SGameCreator.VictoryScore}"
            );
            _winnerText.SetText($"Round Winner: {winner.PlayerName}");
            _winnerText.color = winner.Side.PlayerColor;
            _player1Text.SetText($"{player1.PlayerName}<br><br>Score<br>{player1.Side.Score}");
            _player2Text.SetText($"{player2.PlayerName}<br><br>Score<br>{player2.Side.Score}");
        }

        private void SetCountText(string count)
        {
            _countText.SetText(count);
        }
    }
}
