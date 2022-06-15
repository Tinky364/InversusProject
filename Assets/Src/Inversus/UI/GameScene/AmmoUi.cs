using UnityEngine;
using UnityEngine.UI;

using Inversus.Game;

namespace Inversus.UI.GameScene
{
    public class AmmoUi : MonoBehaviour
    {
        [SerializeField]
        private Gun _gun;

        private Image _img;

        private void Awake()
        {
            _img = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _gun.AmmoChanged.AddListener(CalculateFillAmount);
        }

        private void OnDisable()
        {
            _gun.AmmoChanged.RemoveListener(CalculateFillAmount);
        }

        private void CalculateFillAmount(float current, float max)
        {
            float fillAmount = current / max;
            _img.fillAmount = fillAmount;
        }
    }
}
