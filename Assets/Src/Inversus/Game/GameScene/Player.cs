using UnityEngine;
using UnityEngine.InputSystem;

using Inversus.Attribute;
using Inversus.Helper;

namespace Inversus.Game
{
    public class Player : MonoBehaviour
    {
        [SerializeField, ReadOnly, Expandable]
        private PlayerData _data;
        public PlayerData Data => _data;
        
        [Header("Movement")]
        [SerializeField, Min(0)]
        private float _acceleration = 50f;
        [SerializeField, Min(0)]
        private float _maxSpeed = 6f;
        
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rig;
        private BoxCollider2D _collider;
        private PlayerInput _playerInput;

        private InputAction _moveAction;
        private InputAction _fireAction;

        private Vector2 _moveInputAxis;
        private Vector2 _desiredVelocity;
        private Vector2 _velocity;

        private void OnDisable()
        {
            _moveAction.Disable();
        }

        private void Update()
        {
            GetInputAxis();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        public void Initialize(PlayerData playerData, Vector2 startingPosition)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rig = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _playerInput = GetComponent<PlayerInput>();

            _moveAction = _playerInput.actions["Movement"];
            _fireAction = _playerInput.actions["Fire"];
            _moveAction.Enable();

            _data = playerData;

            gameObject.name = playerData.Name;
            gameObject.layer = HelperMethods.LayerMaskToLayer(_data.Layer);
            _spriteRenderer.color = _data.Color;
            transform.position = startingPosition;
        }
        
        private void GetInputAxis()
        {
            _moveInputAxis = _moveAction.ReadValue<Vector2>();
        }

        private void MovePlayer()
        {
            _desiredVelocity = _maxSpeed * _moveInputAxis;
            _velocity = Vector2.MoveTowards(
                _velocity, _desiredVelocity, _acceleration * Time.fixedDeltaTime
            );
            _rig.velocity = _velocity;
        }
    }
}
