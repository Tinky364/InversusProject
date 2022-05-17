using UnityEngine;

namespace Inversus.Game
{
    public enum SideType { White, Black }
    
    public class Side
    {
        public SideType SideType { get; }
        public int Layer { get; }
        public Color TileColor{ get; }
        public Color PlayerColor { get; }
        public Color BulletColor { get; }
        public Vector2 SpawnPosition { get; }

        public Side(
            SideType sideType, int layer, Color tileColor, Color playerColor, Color bulletColor,
            Vector2 spawnPosition
        )
        {
            SideType = sideType;
            Layer = layer;
            TileColor = tileColor;
            PlayerColor = playerColor;
            BulletColor = bulletColor;
            SpawnPosition = spawnPosition;
        }
    }
}
