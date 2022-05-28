using System.Collections;
using UnityEngine;
using Photon.Pun;

using static Inversus.Facade;

namespace Inversus.Game
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 12;

        public int Id { get; private set; }
        
        public PhotonView PhotonView { get; private set; }
        public Side Side { get; private set; }
        public bool HasSpawned { get; private set; }
        
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rig;
        private BoxCollider2D _collider;
        private LineRenderer _lineRenderer;
        
        private IEnumerator _displayLineRendererCor;
        private Vector2 _moveDirection;
        private Vector2 _velocity;
        private float _lineRendererTailPosition;

        public void Initialize(int id)
        {
            Id = id;
        }
        
        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rig = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _lineRenderer = GetComponentInChildren<LineRenderer>();
            
            SEventBus.RoundEnded.AddListener(UnSpawn);
            SEventBus.GameEnded.AddListener(UnSpawn);
        }

        private void FixedUpdate()
        {
            if (SMainManager.State == States.InGame)
            {
                if (HasSpawned)
                {
                    MoveBullet();
                }
            }
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (SMainManager.State != States.InGame) return;
            if (!HasSpawned) return;

            
            if (col.CompareTag("Wall") || col.CompareTag("Player") || col.CompareTag("Bullet"))
            {
                switch (SGameCreator.GameType)
                {
                    case GameType.Local:
                        StartCoroutine(UnSpawnOnCollision());
                        break;
                    case GameType.Online:
                        if (PhotonNetwork.IsMasterClient)
                            PhotonView.RPC("Execute_UnSpawnOnCollision", RpcTarget.All);
                        break;
                }
            }
        }

        public void SpawnLocal(Vector2 position, Vector2Int direction, Side side)
        {
            HasSpawned = true;
            
            transform.position = position;
            _moveDirection = direction;
            _collider.offset = CalculateColliderOffset(position, direction);
            _lineRenderer.transform.eulerAngles = CalculateLineRendererRotation(direction);
            SetSide(side);
            gameObject.SetActive(true);
            
            if (_displayLineRendererCor != null) StopCoroutine(_displayLineRendererCor);
            _displayLineRendererCor = DisplayLineRendererTail(0, -4, 0.4f);
            StartCoroutine(_displayLineRendererCor);
        }

        public void SpawnOnline(Vector2 position, Vector2Int direction, Side side)
        {
            object[] data = {position, (Vector2)direction, side.Id};
            PhotonView.RPC("Execute_Spawn", RpcTarget.All, data as object);
        }
       
        [PunRPC]
        private void Execute_Spawn(object[] data)
        {
            HasSpawned = true;

            Vector2 position = (Vector2)data[0];
            Vector2Int direction = Vector2Int.RoundToInt((Vector2)data[1]);
            Side side = SGameCreator.Sides[(int)data[2]];
            
            transform.position = position;
            _moveDirection = direction;
            _collider.offset = CalculateColliderOffset(position, direction);
            _lineRenderer.transform.eulerAngles = CalculateLineRendererRotation(direction);
            SetSide(side);
            gameObject.SetActive(true);
            
            if (_displayLineRendererCor != null) StopCoroutine(_displayLineRendererCor);
            _displayLineRendererCor = DisplayLineRendererTail(0, -4, 0.4f);
            StartCoroutine(_displayLineRendererCor);
        }

        private void UnSpawn(int player1Score, int player2Score, string roundWinnerName)
        {
            if (!HasSpawned) return;
            
            HasSpawned = false;
            _rig.velocity = Vector2.zero;
            if (_displayLineRendererCor != null) StopCoroutine(_displayLineRendererCor);
            gameObject.SetActive(false);
            
            SGameSubSceneManager.BulletPool.UnSpawn(this);
        }
        
        [PunRPC]
        private void Execute_UnSpawnOnCollision()
        {
            StartCoroutine(UnSpawnOnCollision());
        }

        private IEnumerator UnSpawnOnCollision()
        {
            if (!HasSpawned) yield break;
            
            HasSpawned = false;
            SetLayer(0); 
            _rig.velocity = Vector2.zero;
            
            if (_displayLineRendererCor != null) StopCoroutine(_displayLineRendererCor);
            _displayLineRendererCor = DisplayLineRendererTail(
                _lineRenderer.GetPosition(1).x, 0, 0.5f
            );
            yield return StartCoroutine(_displayLineRendererCor);

            gameObject.SetActive(false);
            
            SGameSubSceneManager.BulletPool.UnSpawn(this);
        }

        private Vector2 CalculateColliderOffset(Vector2 position, Vector2Int direction)
        {
            const float forwardOffset = 0.12f; 
            
            Vector2 tilePos = Map.GetTilePos(position);
            if (direction == Vector2Int.right) 
                return new Vector2(-forwardOffset, tilePos.y - position.y);
            if (direction == Vector2Int.left) 
                return new Vector2(forwardOffset, tilePos.y - position.y);
            if (direction == Vector2Int.up) 
                return new Vector2(tilePos.x - position.x, -forwardOffset);
            if (direction == Vector2Int.down) 
                return new Vector2(tilePos.x - position.x, forwardOffset);
            
            Debug.LogWarning("Calculating collider offset of a bullet is failed!");
            return Vector2.zero;
        }
        
        private Vector3 CalculateLineRendererRotation(Vector2Int direction)
        {
            if (direction == Vector2Int.right) return new Vector3(0, 0, 0);
            if (direction == Vector2Int.left) return new Vector3(0, 0, 180);
            if (direction == Vector2Int.up) return new Vector3(0, 0, 90);
            if (direction == Vector2Int.down) return new Vector3(0, 0, -90);
            
            Debug.LogWarning("Calculating line Renderer rotation of a bullet is failed!");
            return Vector3.zero;
        }

        private IEnumerator DisplayLineRendererTail(
            float startingValue, float endingValue, float duration
        )
        {
            float dif = Mathf.Abs(startingValue - endingValue);
            if (dif < 4) duration *= dif / 4;
            
            float timeElapsed = 0;
            while (timeElapsed < duration)
            {
                _lineRendererTailPosition = Mathf.Lerp(
                    startingValue, endingValue, timeElapsed / duration
                );
                _lineRenderer.SetPosition(1, new Vector2(_lineRendererTailPosition, 0));
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            _lineRendererTailPosition = endingValue;
            _lineRenderer.SetPosition(1, new Vector2(_lineRendererTailPosition, 0));
        }

        private void SetSide(Side newSide)
        {
            Side = newSide;
            SetLayer(Side.Layer);
            SetColor(Side.BulletColor);
        }
       
        private void SetLayer(int layer) => gameObject.layer = layer;

        private void SetColor(Color color)
        {
            _spriteRenderer.color = color;
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = new Color(color.r, color.g, color.b, 0);
        }

        private void MoveBullet()
        { 
            _velocity = _moveDirection * _speed;
            _rig.velocity = _velocity;
        }
        
        private void OnDestroy()
        {
            SEventBus.RoundEnded.RemoveListener(UnSpawn);
            SEventBus.GameEnded.RemoveListener(UnSpawn);
        }
    }
}
