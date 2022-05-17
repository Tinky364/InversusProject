using UnityEngine;

using static Inversus.Manager.ManagerFacade;

namespace Inversus.Game
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 12;
        
        public Side Side { get; private set; }
        public bool HasSpawned { get; private set; }
        
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rig;
        private BoxCollider2D _collider;
        private Vector2 _moveDirection;
        private Vector2 _velocity;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rig = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
        }

        private void FixedUpdate()
        {
            if (HasSpawned) MoveBullet();
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (SSubSceneManager is not GameSubSceneManager gameSubSceneManager) return;
            
            // Wall
            if (col.gameObject.layer == gameSubSceneManager.GameManager.LayerWall)
            {
                UnSpawn();
                gameSubSceneManager.BulletPool.Push(this);
                return;
            }
            
            // Enemy bullet
            if (col.CompareTag("Bullet"))
            {
                Side oppositeSide = gameSubSceneManager.GameManager.ReturnOppositeSide(Side);
                if (col.gameObject.layer != oppositeSide.Layer) return;
                
                Debug.Log("sa");
                UnSpawn();
                gameSubSceneManager.BulletPool.Push(this);
            }
        }

        public void Spawn(Vector2 position, Vector2 direction, Side side)
        {
            transform.position = position;
            _moveDirection = direction;
            SetSide(side);
            HasSpawned = true;
            gameObject.SetActive(true);
        }

        public void UnSpawn()
        {
            HasSpawned = false;
            _rig.velocity = Vector2.zero;
        }

        private void SetSide(Side newSide)
        {
            Side = newSide;
            SetLayer(Side.Layer);
            SetColor(Side.BulletColor);
        }
       
        private void SetLayer(int layer) => gameObject.layer = layer;

        private void SetColor(Color color) => _spriteRenderer.color = color;
        
        private void MoveBullet()
        { 
            _velocity = _moveDirection * _speed;
            _rig.velocity = _velocity;
        }
    }
}
