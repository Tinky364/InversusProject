using System.Collections;
using UnityEngine;
using Photon.Pun;

using Inversus.Data;
using Inversus.Game;
using Inversus.Helper;

using static Inversus.Facade;

namespace Inversus.Manager
{
    public class GameCreator : SingletonMonoBehaviour<GameCreator>
    {
        [SerializeField]
        private PlayerController _prefabPlayerController;
        
        public Side[] Sides { get; private set; }
        public PlayerController[] PlayerControllers { get; set; }
        public ColorTheme ColorTheme { get; private set; }
        public Map CurrentMap { get; private set; }
        public int CurrentMapId { get; private set; }
        public int LayerSide1 { get; private set; }
        public int LayerSide2 { get; private set; }
        public int VictoryScore { get; private set; }
        public int Round { get; private set; }
        public GameType GameType { get; private set; }
        
        public PhotonView PhotonView { get; private set; }

        private bool _isClientReady;

        protected override void Awake()
        {
            base.Awake();

            PhotonView = GetComponent<PhotonView>();
            SEventBus.PlayerHit.AddListener(OnPlayerHit);
            SEventBus.StartGameRequested.AddListener(SetGameSettings);
            SEventBus.RoundStartRequested.AddListener(OnRoundStartRequested);
            SEventBus.RetryLocalGameRequested.AddListener(RetryGame);
        }

        private void SetGameSettings(int mapId, int victoryScore, int colorThemeId, GameType gameType)
        {
            LayerSide1 = LayerMask.NameToLayer("Side1");
            LayerSide2 = LayerMask.NameToLayer("Side2");
            CurrentMapId = mapId;
            VictoryScore = victoryScore;
            ColorTheme = SDatabase.GetColorTheme(colorThemeId);
            GameType = gameType;
            Sides = new Side[2];
            Sides[0] = new Side(
                0, LayerSide1, ColorTheme.Side1Color, ColorTheme.Side1Color, ColorTheme.Side2Color
            );
            Sides[1] = new Side(
                1, LayerSide2, ColorTheme.Side2Color, ColorTheme.Side2Color, ColorTheme.Side1Color
            );
            PlayerControllers = new PlayerController[2];
            
            Debug.Log($"Map Id: {CurrentMapId}");
            Debug.Log($"Victory Score: {VictoryScore}");
            Debug.Log($"Color Theme Id: {colorThemeId}");
        }

        public void CreateGame()
        {
            switch (GameType)
            {
                case GameType.Local: 
                    CreateGameLocal();
                    break;
                case GameType.Online:
                    if (PhotonNetwork.IsMasterClient)
                        StartCoroutine(CreateGameOnline()); 
                    else
                        PhotonView.RPC("Inform_ClientReady", RpcTarget.MasterClient);
                    break;
            }
        }

#region Online
         private IEnumerator CreateGameOnline()
         {
            while (!_isClientReady) yield return null;
            _isClientReady = false;

            PlayerControllers[0] = PhotonNetwork.Instantiate(
            "PlayerControllerOnline", Vector3.zero, Quaternion.identity
            ).GetComponent<PlayerController>();
            PlayerControllers[0].Initialize(Sides[0], SInputProfileManager.InputProfiles[1]);

            PlayerControllers[1] = PhotonNetwork.Instantiate(
            "PlayerControllerOnline", Vector3.zero, Quaternion.identity
            ).GetComponent<PlayerController>();
            PlayerControllers[1].Initialize(Sides[1], SInputProfileManager.InputProfiles[1]);

            Round = 0;

            object[] data =
            {
                PlayerControllers[0].PhotonView.ViewID,
                PlayerControllers[1].PhotonView.ViewID
            };
            PhotonView.RPC("Send_CreateGame", RpcTarget.Others, data as object);
            
            while (!_isClientReady) yield return null;
            _isClientReady = false;

            Debug.Log("GameCreated Event => Invoke()");
            SEventBus.GameCreated?.Invoke();

            StartCoroutine(CreateRoundOnline());
        }

        [PunRPC]
        private void Send_CreateGame(object[] data)
        {
            PlayerControllers[0] = PhotonView.Find((int)data[0]).GetComponent<PlayerController>();
            PlayerControllers[0].Initialize(Sides[0], SInputProfileManager.InputProfiles[1]);

            PlayerControllers[1] = PhotonView.Find((int)data[1]).GetComponent<PlayerController>();
            PlayerControllers[1].Initialize(Sides[1], SInputProfileManager.InputProfiles[1]);
            
            PlayerControllers[1].PhotonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
            
            Round = 0;
            
            PhotonView.RPC("Inform_ClientReady", RpcTarget.MasterClient);
            
            Debug.Log("GameCreated Event => Invoke()");
            SEventBus.GameCreated?.Invoke();
        }

        private IEnumerator CreateRoundOnline()
        {
            Round += 1;
            
            if (CurrentMap != null) PhotonNetwork.Destroy(CurrentMap.gameObject);
            CurrentMap = PhotonNetwork.Instantiate(
                SDatabase.GetMap(CurrentMapId).name, Vector2.zero, Quaternion.identity
            ).GetComponent<Map>();
            CurrentMap.Initialize(Sides[0], Sides[1]);

            PlayerControllers[0].ResetThis(CurrentMap.SpawnPosition1);
            PlayerControllers[1].ResetThis(CurrentMap.SpawnPosition2);

            object[] data = {CurrentMap.PhotonView.ViewID};
            PhotonView.RPC("Send_CreateRound", RpcTarget.Others, data as object);
            
            while (!_isClientReady) yield return null;
            _isClientReady = false;
            
            Debug.Log("RoundStarted Event => Invoke()");
            SEventBus.RoundStarted?.Invoke();
            
            Time.timeScale = 1;
            SMainManager.State = States.InGame;
        }

        [PunRPC]
        private void Send_CreateRound(object[] data)
        {
            Round += 1;
            
            CurrentMap = PhotonView.Find((int)data[0]).GetComponent<Map>();
            CurrentMap.Initialize(Sides[0], Sides[1]);
            
            PlayerControllers[0].ResetThis(CurrentMap.SpawnPosition1);
            PlayerControllers[1].ResetThis(CurrentMap.SpawnPosition2);
            
            PhotonView.RPC("Inform_ClientReady", RpcTarget.MasterClient);

            Debug.Log("RoundStarted Event => Invoke()");
            SEventBus.RoundStarted?.Invoke();
            
            Time.timeScale = 1;
            SMainManager.State = States.InGame;
        }
        
        [PunRPC]
        private void Inform_ClientReady()
        {
            _isClientReady = true;
        }
#endregion

#region Local
        private void CreateGameLocal()
        {
            PlayerControllers[0] = Instantiate(_prefabPlayerController);
            PlayerControllers[0].Initialize(Sides[0], SInputProfileManager.InputProfiles[1]);

            PlayerControllers[1]= Instantiate(_prefabPlayerController);
            PlayerControllers[1].Initialize(Sides[1], SInputProfileManager.InputProfiles[2]);
            
            Debug.Log("GameCreated Event => Invoke()");
            SEventBus.GameCreated?.Invoke();

            Round = 0;
            CreateRoundLocal();
        }
        
        private void CreateRoundLocal()
        {
            Round += 1;
            
            if (CurrentMap != null) Destroy(CurrentMap.gameObject);
            CurrentMap = Instantiate(
                SDatabase.GetMap(CurrentMapId), Vector2.zero, Quaternion.identity
            );
            CurrentMap.Initialize(Sides[0], Sides[1]);

            PlayerControllers[0].ResetThis(CurrentMap.SpawnPosition1);
            PlayerControllers[1].ResetThis(CurrentMap.SpawnPosition2);

            Debug.Log("RoundStarted Event => Invoke()");
            SEventBus.RoundStarted?.Invoke();
            
            Time.timeScale = 1;
            SMainManager.State = States.InGame;
        }
#endregion

        private void OnPlayerHit(PlayerController playerController)
        {
            Time.timeScale = 0;
            SMainManager.State = States.Loading;
            RoundEnded(playerController);
        }

        private void OnRoundStartRequested()
        {
            CreateRoundLocal();
        }

        private void RoundEnded(PlayerController playerController)
        {
            string roundWinnerName = "";
            if (playerController == PlayerControllers[0])
            {
                roundWinnerName = PlayerControllers[1].InputProfile.name;
                PlayerControllers[1].Side.Score += 1;
            }
            else if (playerController == PlayerControllers[1])
            {
                roundWinnerName = PlayerControllers[1].InputProfile.name;
                PlayerControllers[0].Side.Score += 1;
            }
            Debug.Log($"Round Winner: {roundWinnerName}");

            string winnerName;
            if (PlayerControllers[0].Side.Score == VictoryScore)
            {
                winnerName = PlayerControllers[0].InputProfile.Name;
                Debug.Log($"Game Winner: {winnerName}");
                GameEnded(
                    PlayerControllers[0].Side.Score, PlayerControllers[1].Side.Score,
                    winnerName
                );
            }
            else if (PlayerControllers[1].Side.Score == VictoryScore)
            {
                winnerName = PlayerControllers[1].InputProfile.Name;
                Debug.Log($"Game Winner: {winnerName}");
                GameEnded(
                    PlayerControllers[0].Side.Score, PlayerControllers[1].Side.Score,
                    winnerName
                );
            }
            else
            {
                Debug.Log("RoundEnded Event => Invoke()");
                SEventBus.RoundEnded?.Invoke(
                    PlayerControllers[0].Side.Score, PlayerControllers[1].Side.Score,
                    roundWinnerName
                );
            }
        }

        private void GameEnded(int player1Score, int player2Score, string winnerName)
        {
            Debug.Log("GameEnded Event => Invoke()");
            SEventBus.GameEnded?.Invoke(player1Score, player2Score, winnerName);
        }

        private void RetryGame()
        {
            PlayerControllers[0].Side.Score = 0;
            PlayerControllers[1].Side.Score = 0;
            Round = 0;
            CreateRoundLocal();
        }
        
        private void OnDestroy()
        {
            SEventBus.PlayerHit.RemoveListener(OnPlayerHit);
            SEventBus.StartGameRequested.RemoveListener(SetGameSettings);
            SEventBus.RoundStartRequested.RemoveListener(OnRoundStartRequested);
            SEventBus.RetryLocalGameRequested.RemoveListener(RetryGame);
        }
    }
}
