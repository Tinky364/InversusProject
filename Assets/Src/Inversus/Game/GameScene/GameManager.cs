using System.Collections.Generic;
using UnityEngine;

using Inversus.Attribute;
using Inversus.Manager;
using static Inversus.Manager.ManagerFacade;

namespace Inversus.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Player _prefabPlayer;
        [SerializeField]
        private Camera _mainCam;
        [Header("ALL GAME MAPS")]
        [SerializeField, Expandable]
        private List<Map> _maps = new();
        [Header("COLORS")]
        [SerializeField]
        private Color _colorOfBackground;
        [SerializeField]
        private Color _colorOfWhiteSide;
        [SerializeField]
        private Color _colorOfBlackSide;
        
        public Side SideWhite { get; private set; }
        public Side SideBlack { get; private set; }
        public Player PlayerWhite { get; private set; }
        public Player PlayerBlack { get; private set; }
        public int CurrentMapId { get; private set; }
        public Map CurrentMap { get; private set; }
        public int LayerWall { get; private set; }
        public int LayerWhite { get; private set; }
        public int LayerBlack { get; private set; }
        public int MaxRound 
        { 
            get => _maxRound;
            private set
            {
                if (value < 1) _maxRound = 1;
                else _maxRound = value;
            }
        }
        public int Round
        {
            get => _round;
            set
            {
                if (value < 0) _round = 0;
                else if (value > MaxRound) _round = MaxRound;
                else _round = value;
            }
        }

        private int _maxRound;
        private int _round;
        
        public void Initialize()
        {
            LayerWall = LayerMask.NameToLayer("Wall");
            LayerWhite = LayerMask.NameToLayer("White");
            LayerBlack = LayerMask.NameToLayer("Black");
            SEventBus.PlayerHit.AddListener(OnPlayerHit);
        }

        public void CreateGame(int mapId, int maxRound)
        {
            if (mapId < 0 || mapId >= _maps.Count)
            {
                Debug.LogWarning("The chosen map id is out of range!");
                CurrentMapId = 0;
            }
            else CurrentMapId = mapId;

            SideWhite = new Side(
                SideType.White, LayerWhite, _colorOfWhiteSide, _colorOfBlackSide, _colorOfBlackSide,
                _maps[CurrentMapId].StartingPositionOfWhiteSide
            );
            SideBlack = new Side(
                SideType.Black, LayerBlack, _colorOfBlackSide, _colorOfWhiteSide, _colorOfWhiteSide,
                _maps[CurrentMapId].StartingPositionOfBlackSide
            );
            
            _mainCam.backgroundColor = _colorOfBackground;
            
            PlayerWhite = CreatePlayer("PlayerWhite", SideWhite);
            PlayerBlack = CreatePlayer("PlayerBlack", SideBlack);

            Debug.Log("GameCreated Event => Invoke()");
            SEventBus.GameCreated?.Invoke();

            MaxRound = maxRound;
            Round = 0;
            CreateRound();
        }

        private void CreateRound()
        {
            Round += 1;
            
            if (CurrentMap != null) Destroy(CurrentMap.gameObject);
            CurrentMap = CreateMap(CurrentMapId, SideWhite, SideBlack);

            PlayerWhite.transform.position = SideWhite.SpawnPosition;
            PlayerBlack.transform.position = SideBlack.SpawnPosition;

            Debug.Log("RoundStarted Event => Invoke()");
            SEventBus.RoundStarted?.Invoke();
            
            SMainManager.State = States.InGame;
        }

        private void OnPlayerHit(Player player)
        {
            RoundEnded(player);

            if (Round < MaxRound)
            {
                CreateRound();
            }
            else
            {
                GameEnded();
            }
        }

        private void RoundEnded(Player player)
        {
            SMainManager.State = States.Loading;
            
            if (player == PlayerWhite)
            {
                Debug.Log("Round Loser White");
            }
            else if (player == PlayerBlack)
            {
                Debug.Log("Round Loser Black");
            }
            
            Debug.Log("RoundEnded Event => Invoke()");
            SEventBus.RoundEnded?.Invoke();
        }

        // TODO
        private void GameEnded()
        {
            Round = 0;
            CreateRound();
            
            Debug.Log("GameEnded Event => Invoke()");
            SEventBus.GameEnded?.Invoke();
        }
        
        private Map CreateMap(int mapId, Side sideWhite, Side sideBlack)
        {
            Map map = Instantiate(_maps[mapId], Vector2.zero, Quaternion.identity);
            map.Initialize(sideWhite, sideBlack);
            return map;
        }

        private Player CreatePlayer(string playerName, Side side)
        {
            Player player = Instantiate(_prefabPlayer);
            player.Initialize(playerName, side);
            return player;
        }

        public Side ReturnOppositeSide(Side side)
        {
            switch (side.SideType)
            {
                case SideType.Black: return SideWhite;
                case SideType.White: return SideBlack;
                default: 
                    Debug.LogError("Returning Opposite side is failed!");
                    return side;
            }
        }
        
        private void OnDestroy()
        {
            SEventBus.PlayerHit.RemoveListener(OnPlayerHit);
        }
    }
}
