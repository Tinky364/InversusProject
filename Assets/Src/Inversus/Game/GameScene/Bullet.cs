using UnityEngine;

using static Inversus.Facade;

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
            
            SEventBus.RoundEnded.AddListener(UnSpawn);
            SEventBus.GameEnded.AddListener(UnSpawn);
        }

        private void FixedUpdate()
        {
            if (SMainManager.State == States.InGame)
            {
                if (HasSpawned) MoveBullet();
            }
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (SMainManager.State != States.InGame) return;
            
            if (col.CompareTag("Wall"))
            {
                UnSpawn();
                return;
            }

            if (col.CompareTag("Bullet"))
            {
                Side oppositeSide = SGameCreator.ReturnOppositeSide(Side);
                if (col.gameObject.layer != oppositeSide.Layer) return;
                
                UnSpawn();
            }
        }

        public void Spawn(Vector2 position, Vector2 direction, Side side)
        {
            transform.position = position;
            _collider.offset = Map.GetTilePos(position) - position;
            _moveDirection = direction;
            SetSide(side);
            HasSpawned = true;
            gameObject.SetActive(true);
        }

        public void UnSpawn(int player1Score = 0, int player2Score = 0, string roundWinnerName = "")
        {
            if (!HasSpawned) return;
            
            gameObject.SetActive(false);
            HasSpawned = false;
            _rig.velocity = Vector2.zero;
            if (SSubSceneManager is not GameSubSceneManager gameSubSceneManager) return;
            gameSubSceneManager.BulletPool.Push(this);
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
        
        private void OnDestroy()
        {
            SEventBus.RoundEnded.RemoveListener(UnSpawn);
            SEventBus.GameEnded.RemoveListener(UnSpawn);
        }
    }
}
