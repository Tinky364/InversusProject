using System.Collections.Generic;
using UnityEngine;

using Inversus.Attribute;

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
        
        public int Round { get; private set; }

        public void CreateGame(int mapId)
        {
            LayerWall = LayerMask.NameToLayer("Wall");
            LayerWhite = LayerMask.NameToLayer("White");
            LayerBlack = LayerMask.NameToLayer("Black");
            
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
            
            CurrentMap = CreateMap(CurrentMapId, SideWhite, SideBlack);
            _mainCam.backgroundColor = _colorOfBackground;
            
            PlayerWhite = CreatePlayer("PlayerWhite", SideWhite);
            PlayerBlack = CreatePlayer("PlayerBlack", SideBlack);

            Round = 0;
            CreateRound();
        }

        public void CreateRound()
        {
            Round += 1;
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
    }
}
