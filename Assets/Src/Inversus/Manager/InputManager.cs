using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Inversus.Helper;

using static Inversus.Manager.ManagerFacade;

namespace Inversus.Manager
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class InputManager : SingletonMonoBehaviour<InputManager>
    {
        [SerializeField]
        private Player _prefabPlayer;
        [SerializeField]
        private InputAction _joinAction;
        [SerializeField]
        private InputAction _leaveAction;
        
        public Dictionary<int, Player> Players => _players;
        public int PlayersCount { get; private set; } = 0;

        private PlayerInputManager _playerInputManager;
        private Dictionary<int, Player> _players;

        protected override void Awake()
        {
            base.Awake();

            _playerInputManager = GetComponent<PlayerInputManager>();
            _playerInputManager.playerPrefab = _prefabPlayer.gameObject;

            InitializePlayersDictionary();
            
            _joinAction.Enable();
            _leaveAction.Enable();
            _joinAction.canceled += context => JoinAction(context);
            _leaveAction.canceled += context => LeaveAction(context);
        }

        private void JoinAction(InputAction.CallbackContext context)
        {
            if (SMainManager.State != States.PlayLocallyMenu) return;
            if (PlayersCount >= _playerInputManager.maxPlayerCount) return;
            
            _playerInputManager.JoinPlayerFromActionIfNotAlreadyJoined(context);
        }
        
        private void LeaveAction(InputAction.CallbackContext context)
        {
            if (SMainManager.State != States.PlayLocallyMenu) return;
            if (PlayersCount <= 0) return;

            foreach (KeyValuePair<int, Player> player in Players)
            {
                if (player.Value == null) continue;
                foreach (InputDevice device in player.Value.PlayerInput.devices)
                {
                    if (device != null && context.control.device == device)
                    {
                        Destroy(player.Value.gameObject);
                        return;
                    }
                }
            }
        }

        private void OnPlayerJoined(PlayerInput playerInput)
        {
            int emptyId = ReturnEmptyId();
            if (emptyId == 0)
            {
                Debug.Log("There is no empty slot for a new player!");
                Destroy(playerInput.gameObject);
                return;
            }
            
            Debug.Log($"Player Id {emptyId} joined the game.");

            PlayersCount += 1;
            Player player = playerInput.GetComponent<Player>();
            player.Initialize(emptyId);
            Players[player.Id] = player;
            SSceneCreator.MoveGameObjectToScene(player.gameObject, SSceneCreator.GetManagerScene());
            SEventBus.PlayerJoinedGame?.Invoke(player);
        }

        private void OnPlayerLeft(PlayerInput playerInput)
        {
            if (playerInput.TryGetComponent(out Player player))
            {
                Debug.Log($"Player Id {player.Id} left the game.");

                Players[player.Id] = null;
                PlayersCount -= 1;
            
                SEventBus.PlayerLeftGame?.Invoke(player);
            }
        }

        private int ReturnEmptyId()
        {
            for (int id = 1; id <= Players.Count; id++)
            {
                if (Players[id] == null) return id;
            }
            return 0;
        }

        private void InitializePlayersDictionary()
        {
            _players = new Dictionary<int, Player>();
            for (int id = 1; id <= _playerInputManager.maxPlayerCount; id++)
            {
                _players.Add(id, null);
            }
        }

        public void RemoveAllPlayers()
        {
            for (int id = 1; id <= Players.Count; id++)
            {
                if (Players[id] != null)
                {
                    Destroy(_players[id].gameObject);
                }
            }
        }
    }
}
