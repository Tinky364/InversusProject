﻿using UnityEngine;
using UnityEngine.InputSystem;

namespace Oppositum.Manager
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
    }
}
