using UnityEngine;

using static Inversus.Facade;

namespace Inversus.Game
{
    public class GroundTile : MonoBehaviour
    {
        public Side Side { get; private set; }
        
        private SpriteRenderer _spriteRenderer;

        public void Initialize(string tileName, Side side1, Side side2)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            gameObject.name = tileName;

            if (gameObject.layer == side1.Layer)
            {
                Side = side1;
                SetColor(side1.TileColor);
            }
            else if (gameObject.layer == side2.Layer)
            {
                Side = side2;
                SetColor(side2.TileColor);
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (SMainManager.State != States.InGame) return;
            if (!col.CompareTag("Bullet")) return;

            switch (Side.Id)
            {
                case 1: 
                    SetSide(SGameCreator.Side2);
                    break;
                case 2: 
                    SetSide(SGameCreator.Side1);
                    break;
            }
        }

        private void SetSide(Side newSide)
        {
            Side = newSide;
            SetLayer(Side.Layer);
            SetColor(Side.TileColor);
        }
        
        private void SetLayer(int layer) => gameObject.layer = layer;

        private void SetColor(Color color) => _spriteRenderer.color = color;
    }
}
