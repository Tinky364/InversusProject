using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

using Inversus.Game;

using static Inversus.Facade;

namespace Inversus.UI.GameScene
{
    public class GameEndPanel : Panel
    {
        [SerializeField]
        private TextMeshProUGUI _endText;
        [SerializeField]
        private Image _player1; 
        [SerializeField]
        private Image _player2;
        [SerializeField]
        private Button _playAgainButton;
        [SerializeField]
        private Button _mainMenuButton;

        private TextMeshProUGUI _player1Text;
        private TextMeshProUGUI _player2Text;
        private TextMeshProUGUI _playAgainButtonText;
        private PhotonView _photonView;
        private WaitForSeconds _wfs_1;
        private int _readyCount;
        private bool _isReadyCountChanged = true;
        private bool _isPlayAgainClicked;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _player1Text = _player1.GetComponentInChildren<TextMeshProUGUI>();
            _player2Text = _player2.GetComponentInChildren<TextMeshProUGUI>();
            _playAgainButtonText = _playAgainButton.GetComponentInChildren<TextMeshProUGUI>();
            
            _wfs_1 = new WaitForSeconds(1);
            
            SEventBus.GameEnded.AddListener(OnGameEnded);
        }
        
        private void OnDestroy()
        {
            SEventBus.GameEnded.RemoveListener(OnGameEnded);
        }

        private void OnEnable()
        {
            SEventBus.RoomLeft.AddListener(OnRoomLeft);
            SEventBus.PlayerLeftRoom.AddListener(OnPlayerLeftRoom);
        }

        private void OnDisable()
        {
            SEventBus.RoomLeft.RemoveListener(OnRoomLeft);
            SEventBus.PlayerLeftRoom.RemoveListener(OnPlayerLeftRoom);
        }

        private void Update()
        {
            if (SGameCreator.GameType != GameType.Online) return;
            if (!_isReadyCountChanged) return;
            
            _isReadyCountChanged = false;
            _playAgainButtonText.SetText($"Play Again ({_readyCount}/2)");

            if (_readyCount != 2) return;
            if (!PhotonNetwork.IsMasterClient) return;
            _photonView.RPC("Execute_PlayAgain", RpcTarget.All);
        }

        private void OnGameEnded(
            PlayerController player1, PlayerController player2, PlayerController winner
        )
        {
            SetDisplay(true);
            SetGameEndPanel(player1, player2, winner);
        }
        
        private void OnRoomLeft()
        {
            _readyCount = 0;
            SSceneCreator.LoadScene("MainMenuScene", SubSceneLoadMode.Single);
        }

        private void OnPlayerLeftRoom(Player otherPlayer)
        {
            MainMenuButton_Click();
        }
        
        public void PlayAgainButton_Click()
        {
            switch (SGameCreator.GameType)
            {
                case GameType.Local: 
                    Execute_PlayAgain();
                    break;
                case GameType.Online:
                    if (_isPlayAgainClicked) return;
                    _isPlayAgainClicked = true;
                    _photonView.RPC("Inform_Ready", RpcTarget.All);
                    break;
            }
        }

        [PunRPC]
        private void Inform_Ready()
        {
            _isReadyCountChanged = true;
            _readyCount += 1;
        }

        [PunRPC]
        private void Execute_PlayAgain()
        {
            _isPlayAgainClicked = false;
            _isReadyCountChanged = true;
            _readyCount = 0;
            StartCoroutine(PlayAgainCor());
        }
        
        private IEnumerator PlayAgainCor()
        {
            SetDisplayChildren(false);
            yield return _wfs_1;
            Debug.Log("PlayAgainGameRequested Event => Invoke()");
            SEventBus.PlayAgainGameRequested?.Invoke();
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

        private void SetGameEndPanel(
            PlayerController player1, PlayerController player2, PlayerController winner
        )
        {
            _endText.SetText($"{winner.PlayerName} WINS");
            _endText.color = winner.Side.PlayerColor;
            _player1Text.SetText($"{player1.PlayerName}<br><br>Score<br>{player1.Side.Score}");
            _player2Text.SetText($"{player2.PlayerName}<br><br>Score<br>{player2.Side.Score}");
        }

        private void SetDisplayChildren(bool value)
        {
            _endText.gameObject.SetActive(value);
            _player1.gameObject.SetActive(value);
            _player2.gameObject.SetActive(value);
            _playAgainButton.gameObject.SetActive(value);
            _mainMenuButton.gameObject.SetActive(value);
        }
    }
}
