using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

using static Inversus.Facade;

namespace Inversus.UI.GameScene
{
    public class GameEndPanel : Panel
    {
        [SerializeField]
        private TextMeshProUGUI _endText;
        [SerializeField]
        private TextMeshProUGUI _player1Text;
        [SerializeField]
        private TextMeshProUGUI _player2Text;
        [SerializeField]
        private Button _playAgainButton;
        [SerializeField]
        private Button _mainMenuButton;

        private PhotonView _photonView;
        private WaitForSeconds _waitForSeconds1;
        private bool _isMasterReady;
        private bool _isClientReady;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            
            _waitForSeconds1 = new WaitForSeconds(1);
            
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
            if (!PhotonNetwork.IsMasterClient) return;
            
            if (_isClientReady && _isMasterReady)
            {
                _isClientReady = false;
                _isMasterReady = false;
                _photonView.RPC("Execute_PlayAgain", RpcTarget.All);
            }
        }

        private void OnGameEnded(int player1Score, int player2Score, string winnerName)
        {
            SetDisplay(true);
            SGameCanvasManager.BackgroundPanel.SetDisplay(true);
            SetGameEndPanel(
                player1Score, player2Score, 
                SGameCreator.PlayerControllers[0].PlayerName,
                SGameCreator.PlayerControllers[1].PlayerName,
                winnerName
            );
        }
        
        private void OnRoomLeft()
        {
            _isClientReady = false;
            _isMasterReady = false;
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
                    if (PhotonNetwork.IsMasterClient) _isMasterReady = true;
                    else _photonView.RPC("Inform_ClientReady", RpcTarget.MasterClient);
                    break;
            }
        }

        [PunRPC]
        private void Inform_ClientReady()
        {
            _isClientReady = true;
        }

        [PunRPC]
        private void Execute_PlayAgain()
        {
            StartCoroutine(PlayAgainCor());
        }
        
        private IEnumerator PlayAgainCor()
        {
            SetDisplayChildren(false);
            yield return _waitForSeconds1;
            Debug.Log("PlayAgainGameRequested Event => Invoke()");
            SEventBus.PlayAgainGameRequested?.Invoke();
            SGameCanvasManager.BackgroundPanel.SetDisplay(false);
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
            int playerScore1, int playerScore2, string playerName1, string playerName2,
            string gameWinnerName
        )
        {
            _endText.SetText($"{gameWinnerName} WINS");
            _player1Text.SetText($"{playerName1}<br>Score<br>{playerScore1}");
            _player2Text.SetText($"{playerName2}<br>Score<br>{playerScore2}");
        }

        private void SetDisplayChildren(bool value)
        {
            _endText.gameObject.SetActive(value);
            _player1Text.gameObject.SetActive(value);
            _player2Text.gameObject.SetActive(value);
            _playAgainButton.gameObject.SetActive(value);
            _mainMenuButton.gameObject.SetActive(value);
        }
    }
}
