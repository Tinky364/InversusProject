using Inversus.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

using static Inversus.Manager.ManagerFacade;

namespace Inversus.Game
{
    public class Player : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Min(0)]
        private float _acceleration = 50f;
        [SerializeField, Min(0)]
        private float _maxSpeed = 6f;

        public Side Side { get; private set; }
        
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rig;
        private BoxCollider2D _collider;
        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _rightFireAction;
        private InputAction _leftFireAction;
        private InputAction _upFireAction;
        private InputAction _downFireAction;
        private Vector2 _moveInputAxis;
        private Vector2 _desiredVelocity;
        private Vector2 _velocity;

        private void Update()
        {
            if (SMainManager.State == States.InGame)
            {
                GetInputAxis();
                GetFireInputs();
            }
        }

        private void FixedUpdate()
        {
            if (SMainManager.State == States.InGame)
                MovePlayer();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (SMainManager.State != States.InGame) return;
            if (SSubSceneManager is not GameSubSceneManager gameSubSceneManager) return;

            if (col.CompareTag("Bullet"))
            {
                Side oppositeSide = gameSubSceneManager.GameManager.ReturnOppositeSide(Side);
                if (col.gameObject.layer != oppositeSide.Layer) return;

                col.GetComponent<Bullet>().UnSpawn();
                SEventBus.PlayerHit?.Invoke(this);
            }
        }

        public void Initialize(string playerName, Side side)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rig = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _playerInput = GetComponent<PlayerInput>();

            _moveAction = _playerInput.actions["Movement"];
            _rightFireAction = _playerInput.actions["RightFire"];
            _leftFireAction = _playerInput.actions["LeftFire"];
            _upFireAction = _playerInput.actions["UpFire"];
            _downFireAction = _playerInput.actions["DownFire"];
            _moveAction.Enable();
            _rightFireAction.Enable();
            _leftFireAction.Enable();
            _upFireAction.Enable();
            _downFireAction.Enable();

            gameObject.name = playerName;
            Side = side;
            gameObject.layer = Side.Layer;
            _spriteRenderer.color = Side.PlayerColor;
        }
        
        private void GetInputAxis()
        {
            _moveInputAxis = _moveAction.ReadValue<Vector2>();
        }

        private void GetFireInputs()
        {
            if (_rightFireAction.WasPerformedThisFrame()) FireBullet(Vector2.right);
            else if (_leftFireAction.WasPerformedThisFrame()) FireBullet(Vector2.left);
            else if (_upFireAction.WasPerformedThisFrame()) FireBullet(Vector2.up);
            else if (_downFireAction.WasPerformedThisFrame()) FireBullet(Vector2.down);
        }

        private void MovePlayer()
        {
            _desiredVelocity = _maxSpeed * _moveInputAxis;
            _velocity = Vector2.MoveTowards(
                _velocity, _desiredVelocity, _acceleration * Time.fixedDeltaTime
            );
            _rig.velocity = _velocity;
        }

        private void FireBullet(Vector2 direction)
        {
            if (SSubSceneManager is not GameSubSceneManager gameSubSceneManager) return;

            gameSubSceneManager.BulletPool.Pull().Spawn(
                CalculateSpawnPositionOfBullet(direction), direction, Side
            );
        }

        /// <summary>
        /// It is for not spawning a bullet between 2 tiles.
        /// </summary>
        private Vector2 CalculateSpawnPositionOfBullet(Vector2 direction)
        {
            Vector2 pos = transform.position;
            if (direction == Vector2.right || direction == Vector2.left)
            {
                float yDec = pos.y % 1;
                switch (yDec)
                {
                    case >= 0.35f and <= 0.5f: return new Vector2(pos.x, (int)pos.y + 0.35f);
                    case > 0.5f and <= 0.65f: return new Vector2(pos.x, (int)pos.y + 0.65f);
                }
            }
            else
            {
                float xDec = pos.x % 1;
                switch (xDec)
                {
                    case >= 0.35f and <= 0.5f: return new Vector2((int)pos.x + 0.35f, pos.y);
                    case > 0.5f and <= 0.65f: return new Vector2((int)pos.x + 0.65f, pos.y);
                }
            }
            return pos;
        }
        
        private void OnDisable()
        {
            _moveAction.Disable();
            _rightFireAction.Disable();
            _leftFireAction.Disable();
            _upFireAction.Disable();
            _downFireAction.Disable();
        }
    }
}
