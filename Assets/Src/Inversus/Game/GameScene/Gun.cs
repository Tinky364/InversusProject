using UnityEngine;

using static Inversus.Facade;

namespace Inversus.Game
{
    public class Gun : MonoBehaviour
    {
        public float CurrentAmmo
        {
            get => _currentAmmo;
            private set
            {
                if (value < 0) _currentAmmo = value;
                else if (value > _maxAmmo) _currentAmmo = _maxAmmo;
                else _currentAmmo = value;
            }
        }
        
        private float _currentAmmo;
        private float _loadAmmoTimeElapsed = 0;
        private float _maxAmmo;

        public void Initialize(float maxAmmo)
        {
            _maxAmmo = maxAmmo;
            CurrentAmmo = _maxAmmo;
        }

        public void ResetThis()
        {
            CurrentAmmo = _maxAmmo;
        }

        public void LoadAmmoEverySecond(float second)
        {
            if (CurrentAmmo >= _maxAmmo) return;
            
            if (_loadAmmoTimeElapsed < second) 
            { 
                _loadAmmoTimeElapsed += Time.deltaTime;
                return;
            }
            
            CurrentAmmo += 1;
            _loadAmmoTimeElapsed = 0;
        }

        public void FireBullet(Vector2 spawnerPos, Vector2Int direction, Side side)
        {
            if (CurrentAmmo <= 0) return;
            if (SSubSceneManager is not GameSubSceneManager gameSubSceneManager) return;

            _currentAmmo -= 1;
            gameSubSceneManager.BulletPool.Pull().Spawn(
                CalculateSpawnPositionOfBullet(spawnerPos, direction), direction, side
            );
        }
        
        /// <summary>
        /// It is for not spawning a bullet between 2 tiles.
        /// </summary>
        private Vector2 CalculateSpawnPositionOfBullet(Vector2 spawnerPos, Vector2Int direction)
        {
            const float forwardOffset = 0.3f;
            const float lowerBorder = 0.35f;
            const float upperBorder = 0.65f;
            const float midBorder = 0.5f;
            
            if (direction == Vector2Int.right || direction == Vector2Int.left)
            {
                if (direction == Vector2Int.right) spawnerPos.x += forwardOffset;
                else spawnerPos.x -= forwardOffset;
                
                float yDec = spawnerPos.y % 1;
                switch (yDec)
                {
                    case >= lowerBorder and <= midBorder:
                        return new Vector2(spawnerPos.x, (int)spawnerPos.y + lowerBorder);
                    case > midBorder and <= upperBorder:
                        return new Vector2(spawnerPos.x, (int)spawnerPos.y + upperBorder);
                }
            }
            else if (direction == Vector2Int.up || direction == Vector2Int.down)
            {
                if (direction == Vector2Int.up) spawnerPos.y += forwardOffset;
                else spawnerPos.y -= forwardOffset;
                
                float xDec = spawnerPos.x % 1;
                switch (xDec)
                {
                    case >= lowerBorder and <= midBorder: 
                        return new Vector2((int)spawnerPos.x + lowerBorder, spawnerPos.y);
                    case > midBorder and <= upperBorder: 
                        return new Vector2((int)spawnerPos.x + upperBorder, spawnerPos.y);
                }
            }
            return spawnerPos;
        }
    }
}
