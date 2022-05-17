using UnityEngine;

namespace Inversus.Game
{
    public class WallTile : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        
        public void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = new Color(1, 1, 1, 0);
        }
    }
}
