using System.Collections.Generic;
using UnityEngine;

namespace Inversus.Game
{
    public class Map : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _startingPositionWhite;
        [SerializeField]
        private Vector2 _startingPositionBlack;

        public Vector2 StartingPositionWhite => _startingPositionWhite;
        public Vector2 StartingPositionBlack => _startingPositionBlack;
       
        private readonly Dictionary<string, GroundTile> _tiles = new();

        public void Initialize(Side sideWhite, Side sideBlack)
        {
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
