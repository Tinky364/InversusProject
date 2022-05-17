using System;
using UnityEngine;

namespace Inversus.Game
{
    [Serializable]
    public class Side
    {
        private LayerMask _layer;
        private Color _tileColor;
        private Color _playerColor;

        public LayerMask Layer => _layer;
        public Color TileColor => _tileColor;
        public Color PlayerColor => _playerColor;

        public Side(LayerMask layer, Color tileColor, Color playerColor)
        {
            _layer = layer;
            _tileColor = tileColor;
            _playerColor = playerColor;
        }
    }
}
