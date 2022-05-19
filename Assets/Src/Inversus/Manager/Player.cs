using UnityEngine;
using UnityEngine.InputSystem;

using Inversus.Game;

namespace Inversus.Manager
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private PlayerController _prefabPlayerController;

        public PlayerInput PlayerInput => _playerInput;
        public PlayerController PlayerController { get; private set; }
        public int Id { get; private set; }
        public string Name { get; private set; }

        private PlayerInput _playerInput;
        public InputAction MoveAction { get; private set; }
        public InputAction RightFireAction { get; private set; }
        public InputAction LeftFireAction { get; private set; }
        public InputAction UpFireAction { get; private set; }
        public InputAction DownFireAction { get; private set; }

        public void Initialize(int id)
        {
            _playerInput = GetComponent<PlayerInput>();

            transform.position = Vector2.zero;
            
            Id = id;
            Name = $"User{Id}";
            
            MoveAction = _playerInput.actions["Movement"];
            RightFireAction = _playerInput.actions["RightFire"];
            LeftFireAction = _playerInput.actions["LeftFire"];
            UpFireAction = _playerInput.actions["UpFire"];
            DownFireAction = _playerInput.actions["DownFire"];
        }
        
        public void InitializePlayerController(Side side)
        {
            PlayerController = Instantiate(
                _prefabPlayerController, Vector2.zero, Quaternion.identity, transform
            );
            PlayerController.Initialize(side);
        }

        public void EnableInGameInputs()
        {
            MoveAction.Enable();
            RightFireAction.Enable();
            LeftFireAction.Enable();
            UpFireAction.Enable();
            DownFireAction.Enable();
        }

        public void DisableInGameInputs()
        {
            MoveAction.Disable();
            RightFireAction.Disable();
            LeftFireAction.Disable();
            UpFireAction.Disable();
            DownFireAction.Disable();
        }
    }
}
