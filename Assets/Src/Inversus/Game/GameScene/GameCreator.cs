using System.Collections.Generic;
using UnityEngine;

using Inversus.Attribute;

namespace Inversus.Game
{
    public class GameCreator : MonoBehaviour
    {
        [SerializeField, Expandable]
        private List<Map> _maps = new();
        [SerializeField]
        private Player _prefabPlayer;
        [SerializeField]
        private LayerMask _layerWhite;
        [SerializeField]
        private LayerMask _layerBlack;
        
        private Side _sideWhite;
        private Side _sideBlack;

        private Player _playerWhite;
        public Player PlayerWhite => _playerWhite;
        private Player _playerBlack;
        public Player PlayerBlack => _playerBlack;
        
        private int _currentMapId = 0;
        public int CurrentMapId => _currentMapId;
        private Map _currentMap;
        public Map CurrentMap => _currentMap;

        public void CreateGame()
        {
            _sideWhite = new Side(_layerWhite, Color.white, Color.black);
            _sideBlack = new Side(_layerBlack, Color.black, Color.white);
            
            _currentMap = CreateMap(0, _sideWhite, _sideBlack);
            
            _playerWhite = CreatePlayer(
                1, "PlayerWhite", _sideWhite, CurrentMap.StartingPositionWhite
            );
            _playerBlack = CreatePlayer(
                2, "PlayerBlack", _sideBlack, CurrentMap.StartingPositionBlack
            );
        }
        
        private Map CreateMap(int currentMapId, Side sideWhite, Side sideBlack)
        {
            if (currentMapId < 0 || currentMapId >= _maps.Count)
            {
                Debug.LogWarning("The chosen map id is out of range!");
                _currentMapId = 0;
            }
            else
            {
                _currentMapId = currentMapId;
            }
            Map map = Instantiate(_maps[currentMapId], Vector2.zero, Quaternion.identity);
            map.Initialize(sideWhite, sideBlack);
            return map;
        }

        private Player CreatePlayer(int id, string playerName, Side side, Vector2 startingPosition)
        {
            PlayerData playerData = PlayerData.Create(id, playerName, side.Layer, side.PlayerColor);
            Player player = Instantiate(_prefabPlayer);
            player.Initialize(playerData, startingPosition);
            return player;
        }
    }
}
