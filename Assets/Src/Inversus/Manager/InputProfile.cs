using UnityEngine;
using UnityEngine.InputSystem;

using Inversus.Game;

using static Inversus.Facade;

namespace Inversus.Manager
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputProfile : MonoBehaviour
    {
        public PlayerInput PlayerInput => _playerInput;
        public int Id { get; private set; }
        public string Name { get; private set; }
        public InputAction MoveAction { get; private set; }
        public InputAction RightFireAction { get; private set; }
        public InputAction LeftFireAction { get; private set; }
        public InputAction UpFireAction { get; private set; }
        public InputAction DownFireAction { get; private set; }
        public InputAction PauseAction { get; private set; }

        private PlayerInput _playerInput;

        private void Update()
        {
            if (SMainManager.State == States.InGame)
            {
                GetPauseInput();
            }
        }

        public void Initialize(int id, string profileName)
        {
            _playerInput = GetComponent<PlayerInput>();

            Id = id;
            Name = profileName;
            gameObject.name = Name;
            transform.position = Vector2.zero;

            MoveAction = _playerInput.actions["Movement"];
            RightFireAction = _playerInput.actions["RightFire"];
            LeftFireAction = _playerInput.actions["LeftFire"];
            UpFireAction = _playerInput.actions["UpFire"];
            DownFireAction = _playerInput.actions["DownFire"];
            PauseAction = _playerInput.actions["Pause"];
        }
        
        public void EnableInGameInputs()
        {
            MoveAction.Enable();
            RightFireAction.Enable();
            LeftFireAction.Enable();
            UpFireAction.Enable();
            DownFireAction.Enable();
            PauseAction.Enable();
        }

        public void DisableInGameInputs()
        {
            MoveAction.Disable();
            RightFireAction.Disable();
            LeftFireAction.Disable();
            UpFireAction.Disable();
            DownFireAction.Disable();
            PauseAction.Disable();
        }
        
        private void GetPauseInput()
        {
            if (PauseAction.WasPerformedThisFrame())
            {
                Debug.Log("GamePaused Event => Invoke()");
                SEventBus.GamePaused?.Invoke(this);
            }
        }
    }
}
