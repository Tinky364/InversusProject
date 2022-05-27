using UnityEngine;
using Photon.Pun;

using Inversus.Manager;

using static Inversus.Facade;

namespace Inversus.Game
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Gun))]
    public class PlayerController : MonoBehaviour, IPunObservable
    {
        [Header("Movement")]
        [SerializeField, Min(0)]
        private float _acceleration = 50f;
        [SerializeField, Min(0)]
        private float _maxSpeed = 6f;
        [Header("Gun")]
        [SerializeField, Min(0)]
        private int _maxAmmo = 5;
        [SerializeField, Min(0)]
        private float _ammoLoadDuration = 0.6f;

        public Side Side { get; private set; }
        public InputProfile InputProfile { get; private set; }
        public PhotonView PhotonView { get; private set; }
        public string PlayerName
        {
            get
            {
                if (SGameCreator == null) 
                    return "null";
                if (SGameCreator.GameType == GameType.Local)
                    return InputProfile == null ? "null" : InputProfile.Name;
                if (SGameCreator.GameType == GameType.Online)
                    return PhotonView == null ? "null" : PhotonView.Controller.NickName;
                return "null";
            }
        }

        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rig;
        private Gun _gun;
        private Vector2 _moveInputAxis;
        private Vector2 _desiredVelocity;
        private Vector2 _velocity;
        private Vector3 _networkPosition;
        private float _networkPositionLerpSpeed = 2f;

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rig = GetComponent<Rigidbody2D>();
            _gun = GetComponent<Gun>();
            
            PhotonNetwork.SerializationRate = 30;
        }
        
        public void Initialize(Side side, InputProfile inputProfile)
        {
            InputProfile = inputProfile;
            InputProfile.EnableInGameInputs();

            _gun.Initialize(_maxAmmo);
            Side = side;
            gameObject.name = "PlayerController";
            gameObject.layer = Side.Layer;
            _spriteRenderer.color = Side.PlayerColor;
        }

        private void Update()
        {
            if (SMainManager.State != States.InGame) return;

            switch (SGameCreator.GameType)
            {
                case GameType.Local:
                    GetPauseInput();
                    GetMoveInputAxis();
                    GetFireInputs();
                    _gun.LoadAmmoEverySecond(_ammoLoadDuration);
                    break;
                case GameType.Online:
                {
                    if (PhotonView.IsMine)
                    {
                        GetMoveInputAxis();
                        GetFireInputs();
                        _gun.LoadAmmoEverySecond(_ammoLoadDuration);
                    }
                    break;
                }
            }
        }

        private void FixedUpdate()
        {
            if (SMainManager.State != States.InGame) return;

            switch (SGameCreator.GameType)
            {
                case GameType.Local:
                    MovePlayer();
                    break;
                case GameType.Online:
                    if (PhotonView.IsMine) 
                        MovePlayer();
                    else 
                        SyncMovePlayer();
                    break;
            }
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                Vector3 pos = _rig.position;
                stream.SendNext(pos);
                Vector3 vel = _rig.velocity;
                stream.SendNext(vel);
            }
            else
            {
                Vector3 pos = (Vector3)stream.ReceiveNext();
                _networkPosition = pos;
                Vector3 vel = (Vector3)stream.ReceiveNext();
                _rig.velocity = vel;
                
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                _networkPosition += (Vector3)_rig.velocity * lag;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (SMainManager.State != States.InGame) return;

            if (col.CompareTag("Bullet"))
            {
                SEventBus.PlayerHit?.Invoke(this);
            }
        }

        private void GetMoveInputAxis()
        {
            _moveInputAxis = InputProfile.MoveAction.ReadValue<Vector2>();
        }

        private void GetFireInputs()
        {
            if (InputProfile.RightFireAction.WasPerformedThisFrame()) 
                _gun.FireBullet(transform.position, Vector2Int.right, Side);
            else if (InputProfile.LeftFireAction.WasPerformedThisFrame()) 
                _gun.FireBullet(transform.position, Vector2Int.left, Side);
            else if (InputProfile.UpFireAction.WasPerformedThisFrame()) 
                _gun.FireBullet(transform.position, Vector2Int.up, Side);
            else if (InputProfile.DownFireAction.WasPerformedThisFrame())
                _gun.FireBullet(transform.position, Vector2Int.down, Side);
        }
        
        public void GetPauseInput()
        {
            if (!InputProfile.PauseAction.WasPerformedThisFrame()) return;
            
            Debug.Log("GamePaused Event => Invoke()");
            SEventBus.GamePaused?.Invoke(InputProfile);
        }
        
        public void ResetOnRound(Vector2 spawnPos)
        {
            _rig.velocity = Vector2.zero;
            _moveInputAxis = Vector2.zero;
            _desiredVelocity = Vector2.zero;
            _velocity = Vector2.zero;
            transform.position = spawnPos;
            _gun.ResetOnRound();
        }

        public void Pause()
        {
            _rig.velocity = Vector2.zero;
            _moveInputAxis = Vector2.zero;
            _desiredVelocity = Vector2.zero;
            _velocity = Vector2.zero;
        }

        private void MovePlayer()
        {
            _desiredVelocity = _maxSpeed * _moveInputAxis;
            _velocity = Vector2.MoveTowards(
                _velocity, _desiredVelocity, _acceleration * Time.fixedDeltaTime
            );
            _rig.velocity = _velocity;
        }
        
        private void SyncMovePlayer()
        {
            _rig.position = Vector2.MoveTowards(
                _rig.position, _networkPosition, _networkPositionLerpSpeed * Time.fixedDeltaTime
            );
        }

        private void OnDestroy()
        {
            InputProfile.DisableInGameInputs();
        }
    }
}
