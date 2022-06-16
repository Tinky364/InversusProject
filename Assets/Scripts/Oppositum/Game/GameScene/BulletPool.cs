using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using static Oppositum.Facade;

namespace Oppositum.Game
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField]
        private Bullet _prefabBullet;
        [SerializeField, Min(0)]
        private int _poolSize = 20;
        
        public PhotonView PhotonView { get; private set; }

        private Dictionary<int, Bullet> _pool;
        
        public void Initialize()
        {
            switch (SGameCreator.GameType)
            {
                case GameType.Local:
                {
                    for (int i = 0; i < _poolSize; i++) 
                       ExpandPoolLocal(i);
                    break;
                }
                case GameType.Online:
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                       for (int i = 0; i < _poolSize; i++) 
                           ExpandPoolOnline();
                    }
                    break;
                }
            }
        }

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            _pool = new Dictionary<int, Bullet>();
        }
        
        public void UnSpawn(Bullet bullet)
        {
            bullet.transform.position = new Vector2(-1000, -1000);
        }

        public Bullet Spawn(Vector2 position, Vector2Int direction, Side side)
        {
            switch (SGameCreator.GameType)
            {
                case GameType.Local:
                    return SpawnLocal(position, direction, side);
                case GameType.Online:
                    return SpawnOnline(position, direction, side);
            }
            return null;
        }

        private Bullet SpawnLocal(Vector2 position, Vector2Int direction, Side side)
        {
            foreach (var bulletPair in _pool)
            {
                if (bulletPair.Value.HasSpawned) continue;

                bulletPair.Value.SpawnLocal(position, direction, side);
                return bulletPair.Value;
            }

            Debug.LogWarning("Increase bullet pool size!");
            return null; // TODO fix if needed
        }

        private Bullet SpawnOnline(Vector2 position, Vector2Int direction, Side side)
        {
            foreach (var bulletPair in _pool)
            {
                if (bulletPair.Value.HasSpawned) continue;

                bulletPair.Value.SpawnOnline(position, direction, side);
                return bulletPair.Value;
            }

            Debug.LogWarning("Increase bullet pool size!");
            return null; // TODO fix if needed
        }

        private void ExpandPoolLocal(int id)
        {
            Bullet bullet = Instantiate(_prefabBullet, transform, true);
            bullet.transform.position = new Vector2(-1000, -1000);
            bullet.Initialize(id);
            bullet.gameObject.SetActive(false);
            
            _pool.Add(bullet.Id, bullet);
        }

        private void ExpandPoolOnline()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            Bullet bullet = PhotonNetwork.Instantiate(
                "Bullet", new Vector2(-1000, -1000), Quaternion.identity
            ).GetComponent<Bullet>();
            bullet.transform.SetParent(transform, true);
            bullet.Initialize(bullet.PhotonView.ViewID);
            bullet.gameObject.SetActive(false);
            
            _pool.Add(bullet.Id, bullet);

            object[] data = {bullet.PhotonView.ViewID};
            PhotonView.RPC("Send_ExpandPool", RpcTarget.Others, data as object);
        }

        [PunRPC]
        private void Send_ExpandPool(object[] data)
        {
            Bullet bullet = PhotonView.Find((int)data[0]).GetComponent<Bullet>();
            bullet.transform.SetParent(transform, true);
            bullet.Initialize(bullet.PhotonView.ViewID);
            bullet.gameObject.SetActive(false);
            
            _pool.Add(bullet.Id, bullet);
        }
    }
}
