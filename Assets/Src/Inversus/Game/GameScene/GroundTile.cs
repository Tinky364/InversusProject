using System;
using Photon.Pun;
using UnityEngine;

using static Inversus.Facade;

namespace Inversus.Game
{
    public class GroundTile : MonoBehaviour
    {
        public Side Side { get; private set; }

        private PhotonView _photonView;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(string tileName, Side side1, Side side2)
        {
            gameObject.name = tileName;

            if (gameObject.layer == side1.Layer)
            {
                Side = side1;
                SetColor(side1.TileColor);
            }
            else if (gameObject.layer == side2.Layer)
            {
                Side = side2;
                SetColor(side2.TileColor);
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (SMainManager.State != States.InGame) return;
            if (!col.CompareTag("Bullet")) return;

            switch (SGameCreator.GameType)
            {
                case GameType.Local: 
                    switch (Side.Id)
                    {
                        case 0:
                            SetSide(1);
                            break;
                        case 1:
                            SetSide(0);
                            break;
                    }
                    break;
                case GameType.Online:
                    if (!PhotonNetwork.IsMasterClient) return;
                    switch (Side.Id)
                    {
                        case 0:
                            _photonView.RPC("SetSide", RpcTarget.All, 1);
                            break;
                        case 1:
                            _photonView.RPC("SetSide", RpcTarget.All, 0);
                            break;
                    }
                    break;
            }
        }

        [PunRPC]
        private void SetSide(int id)
        {
            Side = SGameCreator.Sides[id];
            SetLayer(Side.Layer);
            SetColor(Side.TileColor);
        }

        private void SetLayer(int layer) => gameObject.layer = layer;

        private void SetColor(Color color) => _spriteRenderer.color = color;
    }
}
