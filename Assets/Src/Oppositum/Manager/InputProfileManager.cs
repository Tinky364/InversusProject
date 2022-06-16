using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Oppositum.Helper;
using static Oppositum.Facade;

namespace Oppositum.Manager
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class InputProfileManager : SingletonMonoBehaviour<InputProfileManager>
    {
        [SerializeField]
        private InputProfile _prefabInputProfile;
        [SerializeField]
        private InputAction _joinAction;
        [SerializeField]
        private InputAction _leaveAction;
        
        public Dictionary<int, InputProfile> InputProfiles { get; private set; }
        public int InputProfileCount { get; private set; } = 0;
        public int MaxPlayerCount { get; private set; }
        
        private PlayerInputManager _playerInputManager;
        private InputProfile _defaultInputProfile;

        protected override void Awake()
        {
            base.Awake();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _playerInputManager = GetComponent<PlayerInputManager>();
            _playerInputManager.playerPrefab = _prefabInputProfile.gameObject;

            MaxPlayerCount = _playerInputManager.maxPlayerCount;
            InitializeInputProfilesDictionary(MaxPlayerCount);
            
            _joinAction.Enable();
            _leaveAction.Enable();
            _joinAction.canceled += JoinActionExecuted;
            _leaveAction.canceled += LeaveActionExecuted;
        }

        public void Enable(int maxPlayerCount)
        {
            RemoveAllInputProfiles();
            InitializeInputProfilesDictionary(maxPlayerCount);
        }
        
        public void Disable()
        {
            RemoveAllInputProfiles();
            InstantiateDefaultInputProfile();
        }

        private void JoinActionExecuted(InputAction.CallbackContext context)
        {
            if (InputProfileCount >= MaxPlayerCount) return;

            if (SMainManager.State == States.PlayLocallyMenu ||
                (SMainManager.State == States.PlayOnlineMenu && SOnlineManager.InRoom))
            {
                _playerInputManager.JoinPlayerFromActionIfNotAlreadyJoined(context);
            }
        }
        
        private void OnPlayerJoined(PlayerInput playerInput)
        {
            int id = ReturnEmptyId();
            if (id == 0)
            {
                Debug.Log("There is no empty slot for a new Input Profile!");
                Destroy(playerInput.gameObject);
                return;
            }

            InputProfileCount += 1;
            InputProfile inputProfile = playerInput.GetComponent<InputProfile>();
            inputProfile.Initialize(id, $"User{id}");
            InputProfiles[inputProfile.Id] = inputProfile;
            SSceneCreator.MoveGameObjectToScene(
                inputProfile.gameObject, SSceneCreator.GetManagerScene()
            );
            if (id == 1) SCanvasManager.SetUiInputModule(playerInput);
            Debug.Log($"Input Profile joined: {inputProfile.Name}");
            SEventBus.InputProfileJoined?.Invoke(inputProfile);
        }
        
        private void LeaveActionExecuted(InputAction.CallbackContext context)
        {
            if (SMainManager.State != States.PlayLocallyMenu) return;
            if (InputProfileCount <= 0) return;

            foreach (KeyValuePair<int, InputProfile> inputProfile in InputProfiles)
            {
                if (inputProfile.Value == null) continue;
                
                foreach (InputDevice device in inputProfile.Value.PlayerInput.devices)
                {
                    if (device == null || context.control.device != device) continue;
                    
                    Destroy(inputProfile.Value.gameObject);
                    return;
                }
            }
        }

        private void OnPlayerLeft(PlayerInput playerInput)
        {
            if (!playerInput.TryGetComponent(out InputProfile inputProfile)) return;
            
            InputProfiles[inputProfile.Id] = null;
            InputProfileCount -= 1;
            Debug.Log($"Input Profile left: {inputProfile.Name}");
            SEventBus.InputProfileLeft?.Invoke(inputProfile);
        }

        private int ReturnEmptyId()
        {
            for (int id = 1; id <= InputProfiles.Count; id++)
            {
                if (InputProfiles[id] == null) return id;
            }
            return 0;
        }

        private void InitializeInputProfilesDictionary(int maxPlayerCount)
        {
            MaxPlayerCount = maxPlayerCount;
            InputProfiles = new Dictionary<int, InputProfile>();
            for (int id = 1; id <= MaxPlayerCount; id++)
            {
                InputProfiles.Add(id, null);
            }
        }

        public void RemoveAllInputProfiles()
        {
            for (int id = 1; id <= InputProfiles.Count; id++)
            {
                if (InputProfiles[id] != null) Destroy(InputProfiles[id].gameObject);
            }
        }
        
        public void InstantiateDefaultInputProfile()
        {
            if (_defaultInputProfile != null) Destroy(_defaultInputProfile.gameObject);
            _defaultInputProfile = Instantiate(_prefabInputProfile);
            SSceneCreator.MoveGameObjectToScene(
                _defaultInputProfile.gameObject, SSceneCreator.GetManagerScene()
            );
            SCanvasManager.SetUiInputModule(_defaultInputProfile.PlayerInput);
        }
    }
}
