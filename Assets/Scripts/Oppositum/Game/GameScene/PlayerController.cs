using UnityEngine;
using Photon.Pun;
using Oppositum.Attribute;
using Oppositum.Data;
using Oppositum.Manager;
using static Oppositum.Facade;

namespace Oppositum.Game
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(AmmoController))]
    public class PlayerController : MonoBehaviour, IPunObservable
    {
        [Header("MOVEMENT"), SerializeField, Min(0)]
        private float _acceleration = 50f;
        [SerializeField, Min(0)]
        private float _maxSpeed = 6f;
        [Header("GUN"), SerializeField, Min(0)]
        private int _maxAmmo = 5;
        [SerializeField, Min(0)]
        private float _ammoLoadDuration = 0.6f;
        [Header("AUDIO"), SerializeField, Expandable]
        private AudioData _hitAudioData;

        public Side Side { get; private set; }
        public PhotonView PhotonView { get; private set; }
        public string PlayerName
        {
            get
            {
                if (SGameCreator == null) 
                    return "null";
                if (SGameCreator.GameType == GameType.Local)
                    return _inputProfile == null ? "null" : _inputProfile.Name;
                if (SGameCreator.GameType == GameType.Online)
                    return PhotonView == null ? "null" : PhotonView.Controller.NickName;
                return "null";
            }
        }

        private AudioSource _audioSource;
        private InputProfile _inputProfile;
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rig;
        private AmmoController _ammoController;
        private Vector2 _moveInputAxis;
        private Vector2 _desiredVelocity;
        private Vector2 _velocity;
        private Vector3 _networkPosition;
        private const float NetworkPositionLerpSpeed = 2f;

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rig = GetComponent<Rigidbody2D>();
            _ammoController = GetComponent<AmmoController>();
            _audioSource = GetComponent<AudioSource>();
            
            PhotonNetwork.SerializationRate = 30;
        }
        
        public void Initialize(Side side, InputProfile inputProfile)
        {
            _inputProfile = inputProfile;
            _inputProfile.EnableInGameInputs();

            Side = side;
            _ammoController.Initialize(Side, _maxAmmo);
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
                    _ammoController.LoadAmmoEverySecond(_ammoLoadDuration);
                    break;
                case GameType.Online:
                {
                    if (PhotonView.IsMine)
                    {
                        GetMoveInputAxis();
                        GetFireInputs();
                        _ammoController.LoadAmmoEverySecond(_ammoLoadDuration);
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
            if (!col.CompareTag("Bullet")) return;

            switch (SGameCreator.GameType)
            {
                case GameType.Local:
                    Execute_PlayerHit();
                    break;
                case GameType.Online:
                    if (PhotonNetwork.IsMasterClient)
                        PhotonView.RPC("Execute_PlayerHit", RpcTarget.All);
                    break;
            }
        }

        [PunRPC]
        private void Execute_PlayerHit()
        {
            _spriteRenderer.enabled = false;
            _ammoController.ChangeAmmoViewerVisibility(false);
            _hitAudioData.Play(_audioSource);
            SEventBus.PlayerHit?.Invoke(this);
        }

        private void GetMoveInputAxis()
        {
            _moveInputAxis = _inputProfile.MoveAction.ReadValue<Vector2>();
        }

        private void GetFireInputs()
        {
            if (_inputProfile.RightFireAction.WasPerformedThisFrame()) 
                _ammoController.FireBullet(transform.position, Vector2Int.right, Side);
            else if (_inputProfile.LeftFireAction.WasPerformedThisFrame()) 
                _ammoController.FireBullet(transform.position, Vector2Int.left, Side);
            else if (_inputProfile.UpFireAction.WasPerformedThisFrame()) 
                _ammoController.FireBullet(transform.position, Vector2Int.up, Side);
            else if (_inputProfile.DownFireAction.WasPerformedThisFrame())
                _ammoController.FireBullet(transform.position, Vector2Int.down, Side);
        }

        private void GetPauseInput()
        {
            if (!_inputProfile.PauseAction.WasPerformedThisFrame()) return;
            
            Debug.Log("GamePaused Event => Invoke()");
            SEventBus.GamePaused?.Invoke(_inputProfile);
        }
        
        public void ResetOnRound(Vector2 spawnPos)
        {
            _spriteRenderer.enabled = true;
            _ammoController.ChangeAmmoViewerVisibility(true);
            _rig.velocity = Vector2.zero;
            _moveInputAxis = Vector2.zero;
            _desiredVelocity = Vector2.zero;
            _velocity = Vector2.zero;
            transform.position = spawnPos;
            _ammoController.ResetOnRound();
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
                _rig.position, _networkPosition, NetworkPositionLerpSpeed * Time.fixedDeltaTime
            );
        }

        private void OnDestroy()
        {
            _inputProfile.DisableInGameInputs();
        }
    }
}
