using TMPro;
using UnityEngine;

namespace Inversus.UI
{
    public class GameEndPanel : Panel
    {
        [SerializeField]
        private TextMeshProUGUI _endText;
        [SerializeField]
        private TextMeshProUGUI _player1Text;
        [SerializeField]
        private TextMeshProUGUI _player2Text;

        public void SetGameEndPanel(
            int player1Score, int player2Score, string player1Name, string player2Name,
            string gameWinnerName
        )
        {
            _endText.SetText($"{gameWinnerName} WINS");
            _player1Text.SetText($"{player1Name}<br>Score<br>{player1Score}");
            _player2Text.SetText($"{player2Name}<br>Score<br>{player2Score}");
        }
    }
}
