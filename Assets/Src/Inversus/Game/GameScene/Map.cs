using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

using static Inversus.Facade;

namespace Inversus.Game
{
    public class Map : MonoBehaviour
    {
        [SerializeField, Min(1)]
        private int _id;
        [SerializeField]
        private Vector2 _spawnPosition1;
        [SerializeField]
        private Vector2 _spawnPosition2;

        public PhotonView PhotonView { get; private set; }
        public int Id => _id;
        public Vector2 SpawnPosition1 => _spawnPosition1;
        public Vector2 SpawnPosition2 => _spawnPosition2;

        private Dictionary<string, GroundTile> _tiles;

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
        }

        public void Initialize(Side side1, Side side2)
        {
            _tiles = new Dictionary<string, GroundTile>();
            foreach (GroundTile tile in GetComponentsInChildren<GroundTile>())
            {
                Vector2 pos = tile.transform.localPosition;
                string tileName = $"{pos.x},{pos.y}";
                tile.Initialize(tileName, side1, side2);
                _tiles.Add(tileName, tile);
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

        public static Vector2 GetTilePos(Vector2 pos)
        {
            return new Vector2(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        }
    }
}
