using UnityEngine;

using Inversus.Helper;

namespace Inversus.Game
{
    public class GroundTile : MonoBehaviour
    {
        private string _key;
        public string Key => _key;

        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rig;
        private BoxCollider2D _collider;
        
        private Color _color;
        private LayerMask _layer;

        public void Initialize(string keyString, Side sideWhite, Side sideBlack)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rig = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            
            _key = keyString;
            gameObject.name = keyString;
            if (HelperMethods.IsInLayerMask(gameObject, sideWhite.Layer))
                ChangeColor(sideWhite.TileColor);
            if (HelperMethods.IsInLayerMask(gameObject, sideBlack.Layer))
                ChangeColor(sideBlack.TileColor);
        }
        
        public void ChangeLayer(LayerMask layer)
        {
            _layer = layer;
            gameObject.layer = HelperMethods.LayerMaskToLayer(_layer);
        }

        public void ChangeColor(Color newColor)
        {
            _color = newColor;
            _spriteRenderer.color = _color;
        }
    }
}
