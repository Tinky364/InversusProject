using System.Collections.Generic;
using UnityEngine;

namespace Inversus.Game
{
    public class Map : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _startingPositionOfWhiteSide;
        [SerializeField]
        private Vector2 _startingPositionOfBlackSide;

        public Vector2 StartingPositionOfWhiteSide => _startingPositionOfWhiteSide;
        public Vector2 StartingPositionOfBlackSide => _startingPositionOfBlackSide;

        private Dictionary<string, GroundTile> _tiles;

        public void Initialize(Side sideWhite, Side sideBlack)
        {
            _tiles = new Dictionary<string, GroundTile>();
            foreach (GroundTile tile in GetComponentsInChildren<GroundTile>())
            {
                Vector2 pos = tile.transform.localPosition;
                string keyString = $"{pos.x},{pos.y}";
                tile.Initialize(keyString, sideWhite, sideBlack);
                _tiles.Add(keyString, tile);
            }
        }
        
        public GroundTile GetTile(string key)
        {
            if (_tiles.TryGetValue(key, out GroundTile tile)) return tile;
            
            Debug.LogWarning($"Tile with key:{key} is not exist!");
            return null;
        }

        public GroundTile GetTile(int cellX, int cellY)
        {
            return GetTile($"{cellX},{cellY}");
        }
    }
}
