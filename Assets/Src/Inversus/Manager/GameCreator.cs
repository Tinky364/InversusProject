using UnityEngine;

using Inversus.Data;
using Inversus.Game;
using Inversus.Helper;

using static Inversus.Facade;

namespace Inversus.Manager
{
    public class GameCreator : SingletonMonoBehaviour<GameCreator>
    {
        public ColorTheme ColorTheme { get; private set; }
        public Side Side1 { get; private set; }
        public Side Side2 { get; private set; }
        public Player Player1 { get; private set; } // TODO
        public Player Player2 { get; private set; } // TODO
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
            
            LayerSide1 = LayerMask.NameToLayer("Side1");
            LayerSide2 = LayerMask.NameToLayer("Side2");
            
            SEventBus.PlayerHit.AddListener(OnPlayerHit);
            SEventBus.StartLocalGameRequested.AddListener(SetGameSettings);
            SEventBus.RoundStartRequested.AddListener(OnRoundStartRequested);
            SEventBus.RetryLocalGameRequested.AddListener(RetryGame);
        }

        private void SetGameSettings(int mapId, int victoryScore, int colorThemeId)
        {
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

            Player1 = SLocalGameManager.Players[1];
            Player2 = SLocalGameManager.Players[2];
            SSceneCreator.MoveGameObjectToScene(Player1.gameObject, SSceneCreator.GetActiveScene());
            SSceneCreator.MoveGameObjectToScene(Player2.gameObject, SSceneCreator.GetActiveScene());
            Player1.InitializePlayerController(Side1);
            Player2.InitializePlayerController(Side2);

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
            
            Player1.PlayerController.ResetThis(CurrentMap.SpawnPosition1);
            Player2.PlayerController.ResetThis(CurrentMap.SpawnPosition2);

            Debug.Log("RoundStarted Event => Invoke()");
            SEventBus.RoundStarted?.Invoke();
            
            Time.timeScale = 1;
            SMainManager.State = States.InGame;
        }

        private void OnPlayerHit(Player player)
        {
            Time.timeScale = 0;
            SMainManager.State = States.Loading;
            RoundEnded(player);
        }

        private void OnRoundStartRequested()
        {
            CreateRound();
        }

        private void RoundEnded(Player player)
        {
            string roundWinnerName = "";
            if (player == Player1)
            {
                roundWinnerName = Player2.name;
                Player2.PlayerController.Side.Score += 1;
            }
            else if (player == Player2)
            {
                roundWinnerName = Player1.name;
                Player1.PlayerController.Side.Score += 1;
            }
            Debug.Log($"Round Winner: {roundWinnerName}");

            string winnerName;
            if (Player1.PlayerController.Side.Score == VictoryScore)
            {
                winnerName = Player1.Name;
                Debug.Log($"Game Winner: {winnerName}");
                GameEnded(
                    Player1.PlayerController.Side.Score, Player2.PlayerController.Side.Score,
                    winnerName
                );
            }
            else if (Player2.PlayerController.Side.Score == VictoryScore)
            {
                winnerName = Player2.Name;
                Debug.Log($"Game Winner: {winnerName}");
                GameEnded(
                    Player1.PlayerController.Side.Score, Player2.PlayerController.Side.Score,
                    winnerName
                );
            }
            else
            {
                Debug.Log("RoundEnded Event => Invoke()");
                SEventBus.RoundEnded?.Invoke(
                    Player1.PlayerController.Side.Score, Player2.PlayerController.Side.Score,
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
            Player1.PlayerController.Side.Score = 0;
            Player2.PlayerController.Side.Score = 0;
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
