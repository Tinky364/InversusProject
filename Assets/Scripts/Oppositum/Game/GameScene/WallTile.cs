using UnityEngine;

namespace Oppositum.Game
{
    public class WallTile : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        
        public void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}
