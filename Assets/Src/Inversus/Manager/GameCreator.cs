using UnityEngine;

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
        
        public Side Side1 { get; private set; }
        public Side Side2 { get; private set; }
        public PlayerController PlayerController1 { get; private set; }
        public PlayerController PlayerController2 { get; private set; }
        public ColorTheme ColorTheme { get; private set; }
        public Map CurrentMap { get; private set; }
        public int CurrentMapId { get; private set; }
        public int LayerSide1 { get; private set; }
        public int LayerSide2 { get; private set; }
        public int VictoryScore { get; private set; }
        public int Round { get; private set; }
        
        private int _startingMapId;

        protected override void Awake()
        {
            base.Awake();
            
            SEventBus.PlayerHit.AddListener(OnPlayerHit);
            SEventBus.StartLocalGameRequested.AddListener(SetGameSettings);
            SEventBus.RoundStartRequested.AddListener(OnRoundStartRequested);
            SEventBus.RetryLocalGameRequested.AddListener(RetryGame);
        }

        private void SetGameSettings(int mapId, int victoryScore, int colorThemeId)
        {
            LayerSide1 = LayerMask.NameToLayer("Side1");
            LayerSide2 = LayerMask.NameToLayer("Side2");
            _startingMapId = mapId;
            VictoryScore = victoryScore;
            ColorTheme = SDatabase.GetColorTheme(colorThemeId);
            
            Debug.Log($"Map Id: {_startingMapId}");
            Debug.Log($"Victory Score: {VictoryScore}");
            Debug.Log($"Color Theme Id: {colorThemeId}");
        }

        public void CreateGame()
        {
            CurrentMapId = _startingMapId;

            Side1 = new Side(
                1, LayerSide1, ColorTheme.Side1Color, ColorTheme.Side1Color, ColorTheme.Side2Color
            );
            Side2 = new Side(
                2, LayerSide2, ColorTheme.Side2Color, ColorTheme.Side2Color, ColorTheme.Side1Color
            );

            PlayerController1 = Instantiate(_prefabPlayerController);
            PlayerController2 = Instantiate(_prefabPlayerController);
            PlayerController1.Initialize(Side1, SInputProfileManager.InputProfiles[1]);
            PlayerController2.Initialize(Side2, SInputProfileManager.InputProfiles[2]);

            Debug.Log("GameCreated Event => Invoke()");
            SEventBus.GameCreated?.Invoke();

            Round = 0;
            CreateRound();
        }
        
        private void CreateRound()
        {
            Round += 1;
            
            if (CurrentMap != null) Destroy(CurrentMap.gameObject);
            CurrentMap = Instantiate(
                SDatabase.GetMap(CurrentMapId), Vector2.zero, Quaternion.identity
            );
            CurrentMap.Initialize(Side1, Side2);
            
            PlayerController1.ResetThis(CurrentMap.SpawnPosition1);
            PlayerController2.ResetThis(CurrentMap.SpawnPosition2);

            Debug.Log("RoundStarted Event => Invoke()");
            SEventBus.RoundStarted?.Invoke();
            
            Time.timeScale = 1;
            SMainManager.State = States.InGame;
        }

        private void OnPlayerHit(PlayerController playerController)
        {
            Time.timeScale = 0;
            SMainManager.State = States.Loading;
            RoundEnded(playerController);
        }

        private void OnRoundStartRequested()
        {
            CreateRound();
        }

        private void RoundEnded(PlayerController playerController)
        {
            string roundWinnerName = "";
            if (playerController == PlayerController1)
            {
                roundWinnerName = PlayerController2.InputProfile.name;
                PlayerController2.Side.Score += 1;
            }
            else if (playerController == PlayerController2)
            {
                roundWinnerName = PlayerController1.InputProfile.name;
                PlayerController1.Side.Score += 1;
            }
            Debug.Log($"Round Winner: {roundWinnerName}");

            string winnerName;
            if (PlayerController1.Side.Score == VictoryScore)
            {
                winnerName = PlayerController1.InputProfile.Name;
                Debug.Log($"Game Winner: {winnerName}");
                GameEnded(
                    PlayerController1.Side.Score, PlayerController2.Side.Score,
                    winnerName
                );
            }
            else if (PlayerController2.Side.Score == VictoryScore)
            {
                winnerName = PlayerController2.InputProfile.Name;
                Debug.Log($"Game Winner: {winnerName}");
                GameEnded(
                    PlayerController1.Side.Score, PlayerController2.Side.Score,
                    winnerName
                );
            }
            else
            {
                Debug.Log("RoundEnded Event => Invoke()");
                SEventBus.RoundEnded?.Invoke(
                    PlayerController1.Side.Score, PlayerController2.Side.Score,
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
            PlayerController1.Side.Score = 0;
            PlayerController2.Side.Score = 0;
            Round = 0;
            CreateRound();
        }
        
        public Side ReturnOppositeSide(Side side)
        {
            switch (side.Id)
            {
                case 1: return Side2;
                case 2: return Side1;
                default: 
                    Debug.LogError("Returning Opposite side is failed!");
                    return side;
            }
        }
        
        private void OnDestroy()
        {
            SEventBus.PlayerHit.RemoveListener(OnPlayerHit);
            SEventBus.StartLocalGameRequested.RemoveListener(SetGameSettings);
            SEventBus.RoundStartRequested.RemoveListener(OnRoundStartRequested);
            SEventBus.RetryLocalGameRequested.RemoveListener(RetryGame);
        }
    }
}
