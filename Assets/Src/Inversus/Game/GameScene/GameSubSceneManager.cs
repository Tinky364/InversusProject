using UnityEngine;

using Inversus.Manager;

using static Inversus.Manager.ManagerFacade;

namespace Inversus.Game
{
    public class GameSubSceneManager : SubSceneManager
    {
        [SerializeField]
        private GameManager _gameManager;
        [SerializeField]
        private BulletPool _bulletPool;
        
        [SerializeField]
        private int _mapId; // TODO remove it.

        public GameManager GameManager => _gameManager;
        public BulletPool BulletPool => _bulletPool;
        
        protected override void Awake()
        {
            base.Awake();

            _bulletPool.Initialize();
            
            _gameManager.CreateGame(_mapId);
            Debug.Log("GameCreated Event => Invoke()");
            SEventBus?.GameCreated?.Invoke();
        }
    }
}
