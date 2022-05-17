using UnityEngine;

using static Inversus.Manager.ManagerFacade;

namespace Inversus.Game
{
    public class GroundTile : MonoBehaviour
    {
        public string Key { get; private set; }
        
        public Side Side { get; private set; }
        
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rig;
        private BoxCollider2D _collider;

        public void Initialize(string keyString, Side sideWhite, Side sideBlack)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rig = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            
            Key = keyString;
            gameObject.name = keyString;

            if (gameObject.layer == sideWhite.Layer)
            {
                Side = sideWhite;
                SetColor(sideWhite.TileColor);
            }
            else if (gameObject.layer == sideBlack.Layer)
            {
                Side = sideBlack;
                SetColor(sideBlack.TileColor);
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (SSubSceneManager is not GameSubSceneManager gameSubSceneManager) return;
            if (!col.CompareTag("Bullet")) return;
            
            switch (Side.SideType)
            {
                case SideType.White: 
                    SetSide(gameSubSceneManager.GameManager.SideBlack);
                    break;
                case SideType.Black: 
                    SetSide(gameSubSceneManager.GameManager.SideWhite);
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
