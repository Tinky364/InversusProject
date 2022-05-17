using UnityEngine;

using Inversus.Manager;

using static Inversus.Manager.ManagerFacade;

namespace Inversus.Game
{
    public class GameSubSceneManager : SubSceneManager
    {
        [SerializeField]
        private GameCreator _gameCreator;

        protected override void Awake()
        {
            base.Awake();

            _gameCreator.CreateGame();
            Debug.Log("GameCreated Event => Invoke()");
            SEventBus.GameCreated?.Invoke();
        }
    }
}
