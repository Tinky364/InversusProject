using TMPro;
using UnityEngine;

namespace Inversus.UI
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

        public void SetRoundEndPanel(
            int currentRound, int maxRound, int player1Score, int player2Score, 
            string player1Name, string player2Name, string roundWinnerName
        )
        {
            _roundText.SetText($"ROUND {currentRound} - {maxRound}");
            _winnerText.SetText($"Round Winner: {roundWinnerName}");
            _player1Text.SetText($"{player1Name}<br>Score<br>{player1Score}");
            _player2Text.SetText($"{player2Name}<br>Score<br>{player2Score}");
        }

        public void SetCountText(string count)
        {
            _countText.SetText(count);
        }
    }
}
