using UnityEngine;

namespace Oppositum.Game
{
    public class Side
    {
        public int Id { get; }
        public int Layer { get; }
        public Color PlayerColor { get; }
        public Color BulletColor { get; }
        public Color TileColor{ get; }
        public int Score { get; set; }

        public Side(int id, int layer, Color playerColor, Color bulletColor, Color tileColor)
        {
            Id = id;
            Layer = layer;
            PlayerColor = playerColor;
            BulletColor = bulletColor;
            TileColor = tileColor;
            Score = 0;
        }
    }
}
