using System.Collections.Generic;
using UnityEngine;

namespace Inversus.Game
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField]
        private Bullet _prefabBullet;
        
        private Queue<Bullet> _pool;

        public void Initialize()
        {
            _pool = new Queue<Bullet>();
            for (int i = 0; i < 40; i++) 
                ExpandPool();
        }

        public void Push(Bullet bullet)
        {
            bullet.transform.position = new Vector2(-1000, -1000);
            _pool.Enqueue(bullet);
        }

        public Bullet Pull()
        {
            if (_pool.Count <= 0) ExpandPool();
            return _pool.Dequeue();
        }

        private void ExpandPool()
        {
            Bullet bullet = Instantiate(_prefabBullet, transform, true);
            bullet.transform.position = new Vector2(-1000, -1000);
            _pool.Enqueue(bullet);
            bullet.gameObject.SetActive(false);
        }
    }
}
