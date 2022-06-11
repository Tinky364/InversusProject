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
        public PlayerController[] PlayerControllers { get; private set; }
        public ColorTheme ColorTheme { get; private set; }
        public Map CurrentMap { get; private set; }
        public int CurrentMapId { get; private set; }
        public int LayerSide1 { get; private set; }
        public int LayerSide2 { get; private set; }
        public int VictoryScore { get; private set; }
        public int Round { get; private set; }
        public GameType GameType { get; private set; }

        private PhotonView _photonView;
        private bool _isClientReady;

        protected override void Awake()
        {
            base.Awake();

            _photonView = GetComponent<PhotonView>();
            
            SEventBus.PlayerHit.AddListener(OnPlayerHit);
            SEventBus.StartGameRequested.AddListener(SetGameSettings);
            SEventBus.RoundStartRequested.AddListener(OnRoundStartRequested);
            SEventBus.PlayAgainGameRequested.AddListener(PlayAgainGame);
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
                    SGameSubSceneManager.BulletPool.Initialize();
                    break;
                case GameType.Online:
                    if (PhotonNetwork.IsMasterClient) StartCoroutine(CreateGameOnline()); 
                    else _photonView.RPC("Inform_ClientReady", RpcTarget.MasterClient);
                    break;
            }
        }

#region Online
         private IEnumerator CreateGameOnline()
         {
            while (!_isClientReady) yield return null;
            _isClientReady = false;
            
            SGameSubSceneManager.BulletPool.Initialize();

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
            _photonView.RPC("Send_CreateGame", RpcTarget.Others, data as object);
            
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
            
            _photonView.RPC("Inform_ClientReady", RpcTarget.MasterClient);
            
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
            CurrentMap.Initialize(Sides[0], Sides[1], ColorTheme.BackgroundColor);

            PlayerControllers[0].ResetOnRound(CurrentMap.SpawnPosition1);
            PlayerControllers[1].ResetOnRound(CurrentMap.SpawnPosition2);

            object[] data = {CurrentMap.PhotonView.ViewID};
            _photonView.RPC("Send_CreateRound", RpcTarget.Others, data as object);
            
            while (!_isClientReady) yield return null;
            _isClientReady = false;
            
            Debug.Log("RoundReady Event => Invoke()");
            SEventBus.RoundCreated?.Invoke();
        }

        [PunRPC]
        private void Send_CreateRound(object[] data)
        {
            Round += 1;
            
            CurrentMap = PhotonView.Find((int)data[0]).GetComponent<Map>();
            CurrentMap.Initialize(Sides[0], Sides[1], ColorTheme.BackgroundColor);
            
            PlayerControllers[0].ResetOnRound(CurrentMap.SpawnPosition1);
            PlayerControllers[1].ResetOnRound(CurrentMap.SpawnPosition2);
            
            _photonView.RPC("Inform_ClientReady", RpcTarget.MasterClient);

            Debug.Log("RoundCreated Event => Invoke()");
            SEventBus.RoundCreated?.Invoke();
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
            CurrentMap.Initialize(Sides[0], Sides[1], ColorTheme.BackgroundColor);

            PlayerControllers[0].ResetOnRound(CurrentMap.SpawnPosition1);
            PlayerControllers[1].ResetOnRound(CurrentMap.SpawnPosition2);
            
            Debug.Log("RoundCreated Event => Invoke()");
            SEventBus.RoundCreated?.Invoke();
        }
#endregion

        private void OnPlayerHit(PlayerController damagedPlayerController)
        {
            SGameCreator.PlayerControllers[0].Pause();
            SGameCreator.PlayerControllers[1].Pause();
            SMainManager.State = States.Loading;
            
            switch (GameType)
            {
                case GameType.Local:
                    EndRound(damagedPlayerController);
                    break;
                case GameType.Online:
                    _photonView.RPC(
                        "Inform_ClientReady",
                        PhotonNetwork.IsMasterClient ? RpcTarget.Others : RpcTarget.MasterClient
                    );
                    StartCoroutine(OnPlayerHitCor(damagedPlayerController));
                    break;
            }
        }

        private IEnumerator OnPlayerHitCor(PlayerController damagedPlayerController)
        {
            while (!_isClientReady) yield return null;
            _isClientReady = false;
            
            EndRound(damagedPlayerController);
        }
        
        private void EndRound(PlayerController damagedPlayerController)
        {
            int roundWinnerId = 0;
            if (damagedPlayerController == PlayerControllers[0])
            {
                roundWinnerId = 1;
                PlayerControllers[1].Side.Score += 1;
            }
            else if (damagedPlayerController == PlayerControllers[1])
            {
                roundWinnerId = 0;
                PlayerControllers[0].Side.Score += 1;
            }
            Debug.Log($"Round Winner: {PlayerControllers[roundWinnerId].PlayerName}");

            if (PlayerControllers[0].Side.Score == VictoryScore)
            {
                EndGame(
                    PlayerControllers[0], PlayerControllers[1], PlayerControllers[0]
                );
            }
            else if (PlayerControllers[1].Side.Score == VictoryScore)
            {
                EndGame(
                    PlayerControllers[0], PlayerControllers[1], PlayerControllers[1]
                );
            }
            else // There is no winner, start a new round.
            {
                Debug.Log("RoundEnded Event => Invoke()");
                SEventBus.RoundEnded?.Invoke(
                    PlayerControllers[0], PlayerControllers[1], PlayerControllers[roundWinnerId]
                );
            }
        }

        private void OnRoundStartRequested()
        {
            switch (GameType)
            {
                case GameType.Local: 
                    CreateRoundLocal();
                    break;
                case GameType.Online: 
                    if (PhotonNetwork.IsMasterClient) StartCoroutine(CreateRoundOnline());
                    break;
            }
        }

        private void EndGame(PlayerController player1, PlayerController player2, PlayerController winner)
        {
            Debug.Log($"Game Winner: {winner.PlayerName}");
            Debug.Log("GameEnded Event => Invoke()");
            SEventBus.GameEnded?.Invoke(player1, player2, winner);
        }

        private void PlayAgainGame()
        {
            PlayerControllers[0].Side.Score = 0;
            PlayerControllers[1].Side.Score = 0;
            Round = 0;

            switch (GameType)
            {
                case GameType.Local:
                    CreateRoundLocal();
                    break;
                case GameType.Online: 
                    if (PhotonNetwork.IsMasterClient) StartCoroutine(CreateRoundOnline());
                    break;
            }
        }
        
        private void OnDestroy()
        {
            SEventBus.PlayerHit.RemoveListener(OnPlayerHit);
            SEventBus.StartGameRequested.RemoveListener(SetGameSettings);
            SEventBus.RoundStartRequested.RemoveListener(OnRoundStartRequested);
            SEventBus.PlayAgainGameRequested.RemoveListener(PlayAgainGame);
        }
    }
}
